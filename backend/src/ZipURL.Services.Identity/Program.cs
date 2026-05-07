using System.Text;
using ZipURL.Services.Identity.Common;
using ZipURL.Services.Identity.Data;
using ZipURL.Services.Identity.Models;
using ZipURL.Services.Identity.Services.Implementations;
using ZipURL.Services.Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration =====
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

// ===== Database =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Services =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ===== Controllers =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:3000",
                "https://zip-url-app.vercel.app",
                "https://zip-url-9velqzf6g-tatin3469-gmailcoms-projects.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ===== JWT Authentication =====
var jwtOptions = builder.Configuration
    .GetSection("Jwt").Get<JwtOptions>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // ĐỌC TOKEN TỪ COOKIE thay vì Header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Ưu tiên đọc từ cookie
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ===== Build app =====
var app = builder.Build();

app.Use(async (context, next) =>
{
    // Ghi lại thời gian bắt đầu request
    var startTime = DateTime.UtcNow;
    
    await next();

    // Kiểm tra nếu Status code nằm trong khoảng 200-299 (Thành công)
    if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
    {
        var elapsed = DateTime.UtcNow - startTime;
        Console.WriteLine($"✅ [SUCCESS] {context.Request.Method} {context.Request.Path} | Status: {context.Response.StatusCode} | Time: {elapsed.TotalMilliseconds}ms");
    }
});

// ===== Middleware pipeline =====
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
// move UseHttpsRedirection  from 117 to here
}

app.UseRouting();
app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
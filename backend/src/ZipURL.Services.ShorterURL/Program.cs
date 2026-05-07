using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Features;
    
var builder = WebApplication.CreateBuilder(args);

// Database Postgres Neon
builder.Services.AddDbContext<URLAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis Upstash
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ZipURL_";
});
// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://zip-url-app.vercel.app")
            .WithOrigins(allowedOrigins) 
            .SetIsOriginAllowed(origin => 
            {
                var host = new Uri(origin).Host;
                return host == "localhost" || host.EndsWith(".vercel.app");
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is missing");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Read token from header or cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                context.Token = authHeader.Substring("Bearer ".Length).Trim();
            }
            else
            {
                context.Token = context.Request.Cookies["access_token"];
            }
            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization(); 

// Services
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseCors("frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapShorterURLEndpoints();

app.MapControllers();

// Database migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<URLAppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

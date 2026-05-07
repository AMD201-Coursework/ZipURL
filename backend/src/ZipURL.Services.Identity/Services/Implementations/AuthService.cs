using ZipURL.Services.Identity.Common;
using ZipURL.Services.Identity.Data;
using ZipURL.Services.Identity.DTOs.Requests;
using ZipURL.Services.Identity.DTOs.Responses;
using ZipURL.Services.Identity.Models;
using ZipURL.Services.Identity.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ZipURL.Services.Identity.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _hasher;
    private readonly JwtOptions _jwt;

    public AuthService(
        AppDbContext db,
        ITokenService tokenService,
        IPasswordHasher<User> hasher,
        IOptions<JwtOptions> jwt)
    {
        _db = db;
        _tokenService = tokenService;
        _hasher = hasher;
        _jwt = jwt.Value;
    }

    // ===== REGISTER =====
    public Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        RegisterAsync(RegisterRequest request)
    {
        return RegisterAsyncImpl(request);
    }

    private async Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        RegisterAsyncImpl(RegisterRequest request)
    {
        // 1. Kiểm tra email đã tồn tại chưa
        var emailExists = await _db.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
            throw new ApplicationException("Email đã được sử dụng");

        // 2. Tạo user mới
        var user = new User
        {
            Email = request.Email.Trim().ToLower(),
            DisplayName = request.DisplayName.Trim(),
            Role = "User"
        };

        // 3. Hash password
        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        // 4. Lưu user
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // 5. Tạo token và trả về
        return await IssueTokensAsync(user);
    }

    // ===== LOGIN =====
    public async Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        LoginAsync(LoginRequest request)
    {
        // 1. Tìm user
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.Trim().ToLower());

        if (user == null)
            throw new ApplicationException("Email hoặc mật khẩu không đúng");

        // 2. Verify password
        var result = _hasher.VerifyHashedPassword(
            user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new ApplicationException("Email hoặc mật khẩu không đúng");

        // 3. Tạo token và trả về
        return await IssueTokensAsync(user);
    }

    // ===== REFRESH =====
    public async Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        RefreshAsync(string refreshToken)
    {
        // 1. Hash token gửi lên để so sánh với DB
        var tokenHash = _tokenService.HashToken(refreshToken);

        // 2. Tìm refresh token hợp lệ
        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == tokenHash &&
                rt.RevokedAt == null &&
                rt.ExpiresAt > DateTime.UtcNow);

        if (storedToken == null)
            throw new ApplicationException("Refresh token không hợp lệ hoặc đã hết hạn");

        // 3. Revoke token cũ
        storedToken.RevokedAt = DateTime.UtcNow;

        // 4. Cấp token mới
        var result = await IssueTokensAsync(storedToken.User);

        await _db.SaveChangesAsync();

        return result;
    }

    // ===== LOGOUT =====
    public async Task LogoutAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return;

        var tokenHash = _tokenService.HashToken(refreshToken);

        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == tokenHash &&
                rt.RevokedAt == null);

        if (storedToken != null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    // ===== HELPER: Tạo cặp token mới =====
    private async Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        IssueTokensAsync(User user)
    {
        // Tạo access token
        var accessToken = _tokenService.CreateAccessToken(user);

        // Tạo refresh token
        var rawRefreshToken = _tokenService.CreateRefreshToken();

        // Lưu refresh token vào DB (hash)
        var refreshEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(rawRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays)
        };

        _db.RefreshTokens.Add(refreshEntity);
        await _db.SaveChangesAsync();

        // Tạo response
        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes)
        };

        return (response, accessToken, rawRefreshToken);
    }
}
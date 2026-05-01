using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZipURL.Services.Identity.Common;
using ZipURL.Services.Identity.DTOs.Requests;
using ZipURL.Services.Identity.DTOs.Responses;
using ZipURL.Services.Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ZipURL.Services.Identity.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtOptions _jwt;

    public AuthController(IAuthService authService, IOptions<JwtOptions> jwt)
    {
        _authService = authService;
        _jwt = jwt.Value;
    }

    // ===== REGISTER =====
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var (response, accessToken, refreshToken) = await _authService.RegisterAsync(request);

        // Set token vào cookie
        CookieTokenHelper.SetTokenCookies(
            Response, accessToken, refreshToken,
            _jwt.AccessTokenMinutes, _jwt.RefreshTokenDays);

        return Ok(response);
    }

    // ===== LOGIN =====
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var (response, accessToken, refreshToken) = await _authService.LoginAsync(request);

        CookieTokenHelper.SetTokenCookies(
            Response, accessToken, refreshToken,
            _jwt.AccessTokenMinutes, _jwt.RefreshTokenDays);

        return Ok(response);
    }

    // ===== REFRESH =====
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        // Đọc refresh token từ cookie
        var refreshToken = CookieTokenHelper.GetRefreshToken(Request);

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { error = "Không tìm thấy refresh token" });

        var (response, newAccessToken, newRefreshToken) =
            await _authService.RefreshAsync(refreshToken);

        CookieTokenHelper.SetTokenCookies(
            Response, newAccessToken, newRefreshToken,
            _jwt.AccessTokenMinutes, _jwt.RefreshTokenDays);

        return Ok(response);
    }

    // ===== LOGOUT =====
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Đọc refresh token từ cookie
        var refreshToken = CookieTokenHelper.GetRefreshToken(Request);

        // Revoke trong DB
        await _authService.LogoutAsync(refreshToken ?? "");

        // Xóa cookie
        CookieTokenHelper.ClearTokenCookies(Response);

        return NoContent();
    }

    // ===== GET CURRENT USER =====
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
                 ?? User.FindFirstValue(ClaimTypes.Email);
        var displayName = User.FindFirstValue("display_name");
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            UserId = userId,
            Email = email,
            DisplayName = displayName,
            Role = role
        });
    }
}
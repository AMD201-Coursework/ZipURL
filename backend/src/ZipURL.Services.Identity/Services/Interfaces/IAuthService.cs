using ZipURL.Services.Identity.DTOs.Requests;
using ZipURL.Services.Identity.DTOs.Responses;

namespace ZipURL.Services.Identity.Services.Interfaces;

public interface IAuthService
{
    Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        RegisterAsync(RegisterRequest request);

    Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        LoginAsync(LoginRequest request);

    Task<(AuthResponse Response, string AccessToken, string RefreshToken)>
        RefreshAsync(string refreshToken);

    Task LogoutAsync(string? refreshToken);
}
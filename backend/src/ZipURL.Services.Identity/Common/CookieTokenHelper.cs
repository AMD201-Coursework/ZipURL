namespace ZipURL.Services.Identity.Common;

/// <summary>
/// Helper để set/xóa token cookie
/// Tập trung logic cookie vào 1 chỗ
/// </summary>
public static class CookieTokenHelper
{
    private const string AccessTokenCookie = "access_token";
    private const string RefreshTokenCookie = "refresh_token";

    public static void SetTokenCookies(
        HttpResponse response,
        string accessToken,
        string refreshToken,
        int accessTokenMinutes,
        int refreshTokenDays)
    {
        // Access token cookie
        response.Cookies.Append(AccessTokenCookie, accessToken, new CookieOptions
        {
            HttpOnly = true,       // JS không đọc được → chống XSS
            Secure = true,         // Chỉ gửi qua HTTPS
            SameSite = SameSiteMode.None, // Cho phép cross-site (frontend ≠ backend domain)
            Expires = DateTimeOffset.UtcNow.AddMinutes(accessTokenMinutes),
            Path = "/"
        });

        // Refresh token cookie
        response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenDays),
            Path = "/api/auth"     // Chỉ gửi khi gọi auth endpoint
        });
    }

    public static void ClearTokenCookies(HttpResponse response)
    {
        response.Cookies.Delete(AccessTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        response.Cookies.Delete(RefreshTokenCookie, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/api/auth"
        });
    }

    public static string? GetAccessToken(HttpRequest request)
    {
        return request.Cookies[AccessTokenCookie];
    }

    public static string? GetRefreshToken(HttpRequest request)
    {
        return request.Cookies[RefreshTokenCookie];
    }
}
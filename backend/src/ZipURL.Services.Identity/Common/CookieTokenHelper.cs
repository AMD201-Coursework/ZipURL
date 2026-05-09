namespace ZipURL.Services.Identity.Common;

/// Helper to set/clear token cookies
/// Centralizes cookie logic in one place

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
            HttpOnly = true,       // JS cannot read → prevents XSS
            Secure = true,         // Only sent over HTTPS
            SameSite = SameSiteMode.None, // Allows cross-site (frontend ≠ backend domain)
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
            Path = "/api/auth"     // Only send when calling auth endpoint
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
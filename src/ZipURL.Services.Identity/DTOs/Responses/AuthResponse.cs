namespace ZipURL.Services.Identity.DTOs.Responses;

/// <summary>
/// Trả về sau khi login/register/refresh thành công
/// Token thực tế nằm trong cookie, response chỉ chứa metadata
/// </summary>
public class AuthResponse
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime AccessTokenExpiresAt { get; set; }
}
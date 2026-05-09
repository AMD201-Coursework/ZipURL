namespace ZipURL.Services.Identity.DTOs.Responses;

public class AuthResponse
{
    public int UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime AccessTokenExpiresAt { get; set; }
}
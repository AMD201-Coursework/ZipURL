using System.ComponentModel.DataAnnotations;

namespace ZipURL.Services.Identity.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6)]
    [MaxLength(128)]
    public string Password { get; set; } = default!;

    [Required]
    [MaxLength(128)]
    public string DisplayName { get; set; } = default!;
}
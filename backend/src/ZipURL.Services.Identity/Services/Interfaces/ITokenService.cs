using ZipURL.Services.Identity.Models;

namespace ZipURL.Services.Identity.Services.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
    string HashToken(string token);
}
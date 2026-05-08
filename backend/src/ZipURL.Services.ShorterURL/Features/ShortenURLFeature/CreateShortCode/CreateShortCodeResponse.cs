namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.CreateShortCode
{
    public record CreateShortCodeResponse(
        string ShortCode,
        string OriginalUrl,
        DateTime CreateAt
        );
}

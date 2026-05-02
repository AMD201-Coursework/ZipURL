using ZipURL.Services.ShorterURL.Features.ShortenURLFeature;
namespace ZipURL.Services.ShorterURL.Features
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapShorterURLEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/shortcode");

            // Gọi Map của Feature Create
            ShortenURLFeature.CreateShortCode.Endpoint.Map(group);

            ShortenURLFeature.GetUserURLs.Endpoint.Map(group);

            ShortenURLFeature.ChangeUrlStatus.Endpoint.Map(group);

            return app;
        }
    }
}

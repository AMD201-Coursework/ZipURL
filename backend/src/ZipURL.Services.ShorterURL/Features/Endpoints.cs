namespace ZipURL.Services.ShorterURL.Features
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapShorterURLEndpoints(this IEndpointRouteBuilder app)
        {
            // Create short code group endpoint
            var group = app.MapGroup("/api/shortcode");

            // Create short code
            ShortenURLFeature.CreateShortCode.Endpoint.Map(group);

            // Get urls of user
            ShortenURLFeature.GetUserURLs.Endpoint.Map(group);

            // Change url status
            ShortenURLFeature.ChangeUrlStatus.Endpoint.Map(group);

            // Redirect
            ShortenURLFeature.Redirect.Endpoint.Map(app);

            return app;
        }
    }
}

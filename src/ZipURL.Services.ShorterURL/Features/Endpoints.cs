namespace ZipURL.Services.ShorterURL.Features
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapShorterURLEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/shortcode");

            // Create
            ShortenURLFeature.CreateShortCode.Endpoint.Map(group);

            // Get URLs 
            ShortenURLFeature.GetUserURLs.Endpoint.Map(group);

            // Change Url Status 
            ShortenURLFeature.ChangeUrlStatus.Endpoint.Map(group);

            // Redirect
            ShortenURLFeature.Redirect.Endpoint.Map(app);

            return app;
        }
    }
}

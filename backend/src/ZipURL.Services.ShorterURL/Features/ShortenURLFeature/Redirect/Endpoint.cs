using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Helpers;

namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.Redirect
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/{shortCode}", async (
                string shortCode,
                URLAppDbContext db,
                IDistributedCache cache) =>
            {
                // Check REDIS
                string cachedUrl = await cache.GetStringAsync(shortCode);
                if (!string.IsNullOrEmpty(cachedUrl))
                {
                    // Cache save "DISABLED" if link is disabled
                    if (cachedUrl == "DISABLED")
                        return Results.NotFound(new { message = "Link is disabled." });

                    return Results.Redirect(cachedUrl);
                }

                // Decode ShortCode
                var id = HashidHelper.Decode(shortCode);
                if (id <= 0)
                    return Results.NotFound(new { message = "Invalid or non-existent shortcode." });

                // Query DATABASE
                var urlItem = await db.URLItems.FindAsync(id);
                if (urlItem == null)
                    return Results.NotFound(new { message = "URL does not exist on the system." });

                // Check IsActive
                if (!urlItem.IsActive)
                {
                    // Cache disabled 1 hour to avoid DB query
                    var disabledOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                    await cache.SetStringAsync(shortCode, "DISABLED", disabledOptions);

                    return Results.NotFound(new { message = "Link is disabled." });
                }

                // Cache new URL
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(3));
                await cache.SetStringAsync(shortCode, urlItem.OriginalUrl, cacheOptions);

                // Redirect
                return Results.Redirect(urlItem.OriginalUrl);
            })
            .WithName("RedirectToOriginal");
        }
    }
}
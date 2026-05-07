using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.ChangeUrlStatus
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder group)
        {
            // Change url status endpoint
            group.MapPatch("/{id}/status", async (int id, URLAppDbContext db, IDistributedCache cache) =>
            {
                var urlItem = await db.URLItems.FindAsync(id);

                if (urlItem is null)
                    return Results.NotFound(new { message = "Not Found." });

                urlItem.IsActive = !urlItem.IsActive;
                await db.SaveChangesAsync();

                // Remove cache to redirect endpoint read new status from DB
                if (!string.IsNullOrEmpty(urlItem.ShortCode))
                    await cache.RemoveAsync(urlItem.ShortCode);

                string notice = urlItem.IsActive ? "Url activated successfully" : "Url deactivated successfully";

                return Results.Ok(new
                {
                    id = urlItem.Id,
                    isActive = urlItem.IsActive,
                    message = notice
                });
            })
            .WithName("UpdateUrlActiveStatus");
        }
    }
}
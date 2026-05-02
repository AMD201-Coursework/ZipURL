using Microsoft.EntityFrameworkCore;
using ZipURL.Services.ShorterURL.Data;

namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.GetUserURLs
{
    public static class Endpoint
    {
        public static void Map(IEndpointRouteBuilder group)
        {
            group.MapGet("/{userId}", async (string userId, URLAppDbContext db) =>
            {
                var urls = await db.URLItems
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Results.Ok(urls);
            })
            .WithName("GetUserURLs");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Helpers;
using ZipURL.Services.ShorterURL.Models;


namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.CreateShortCode
{
    public class Endpoint
    {

        public static void Map(IEndpointRouteBuilder group)
        {
            group.MapPost("/", async (
                CreateShortCodeRequest req,
                URLAppDbContext db,
                IDistributedCache cache,
                // ClaimsPrincipal user,
                [FromServices] IHttpClientFactory httpClientFactory) =>
            {
                // Get User Id
                var userId = req.UserId;

                // Format url check
                if (string.IsNullOrWhiteSpace(req.TargetUrl) || !Uri.TryCreate(req.TargetUrl, UriKind.Absolute, out var uriResult))
                {
                    return Results.BadRequest(new { message = "Invalid URL format." });
                }

                // Check REDIS
                string originKey = $"origin:{req.TargetUrl}";
                string existingShortCode = await cache.GetStringAsync(originKey);

                if (!string.IsNullOrEmpty(existingShortCode))
                {
                    return Results.Conflict(new
                    {
                        message = "You have already shortened this URL (cache).",
                        shortCode = existingShortCode
                    });
                }

                // HEAD request
                try
                {
                    var client = httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var responseCheck = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, req.TargetUrl));

                    if (!responseCheck.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { message = "The URL provided is unreachable." });
                    }
                }
                catch { /* Log error if needed */ }

                var existingLink = await db.URLItems
            .FirstOrDefaultAsync(x => x.OriginalUrl == req.TargetUrl && x.UserId == userId);

                if (existingLink != null)
                {
                    return Results.Conflict(new { message = "Already shortened.", shortCode = existingLink.ShortCode });
                }

                // Create url item
                var urlItem = new URLItem
                {
                    OriginalUrl = req.TargetUrl,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ShortCode = null,
                };

                db.URLItems.Add(urlItem);
                await db.SaveChangesAsync();

                // HashId encode
                urlItem.ShortCode = HashidHelper.Encode(urlItem.Id);
                await db.SaveChangesAsync();

                return Results.Created($"/api/shortcode/{urlItem.ShortCode}",
                    new CreateShortCodeResponse(urlItem.ShortCode, urlItem.OriginalUrl, urlItem.CreatedAt));
            })
    .WithName("CreateShortLink")
    .RequireAuthorization();
        }
    }
}

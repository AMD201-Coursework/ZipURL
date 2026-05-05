using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Helpers;
using ZipURL.Services.ShorterURL.Models;


namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.CreateShortCode
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder group)
        {   //  Define post "/" 
            group.MapPost("/", async (CreateShortCodeRequest req, URLAppDbContext db,
                IDistributedCache cache,
                [FromServices] IHttpClientFactory httpClientFactory) =>
            {
                // Format url check
                if (string.IsNullOrWhiteSpace(req.TargetUrl) || !Uri.TryCreate(req.TargetUrl, UriKind.Absolute, out var uriResult))
                {
                    return Results.BadRequest(new { message = "Invalid URL format." });
                }



                // --- 2. CHECK REDIS NGAY TỪ ĐẦU (Sử dụng URL dài làm Key) ---
                // Ta thêm tiền tố "origin:" để phân biệt với Key của mã ngắn
                string originKey = $"origin:{req.TargetUrl}";
                string existingShortCode = await cache.GetStringAsync(originKey);

                if (!string.IsNullOrEmpty(existingShortCode))
                {
                    // Nếu Redis đã có, trả về luôn mã ngắn đó
                    return Results.Conflict(new
                    {
                        message = "You have already shortened this URL (cache).",
                        shortCode = existingShortCode
                    });
                }

                // Send HEAD res to check the url
                try
                {
                    var client = httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(5); // timeout

                    var request = new HttpRequestMessage(HttpMethod.Head, req.TargetUrl);
                    var responseCheck = await client.SendAsync(request);

                    if (!responseCheck.IsSuccessStatusCode)
                    {
                        return Results.BadRequest(new { message = "The URL provided does not exist or is unreachable." });
                    }
                }
                catch (Exception)
                {
                    return Results.BadRequest(new { message = "Could not verify the URL. Please check if the link is correct." });
                }

                // Duplicate check
                var existingLink = await db.URLItems.FirstOrDefaultAsync(x => x.OriginalUrl == req.TargetUrl && x.UserId == req.UserId);

                if (existingLink != null)
                {
                    // Return 409 Conflict and old ShortCode 
                    return Results.Conflict(new
                    {
                        message = "You have already shortened this URL.",
                        shortCode = existingLink.ShortCode
                    });
                }

                // Create first entity
                var urlItem = new URLItem
                {
                    OriginalUrl = req.TargetUrl,
                    UserId = req.UserId,
                    CreatedAt = DateTime.UtcNow,
                    ShortCode = null,
                };

                // Save to get an Id
                db.URLItems.Add(urlItem);
                await db.SaveChangesAsync();

                // Use Helper Hashids to create ShortCode
                urlItem.ShortCode = HashidHelper.Encode(urlItem.Id);

                // Save with ShortCode
                await db.SaveChangesAsync();

                // Return DTO 
                var response = new CreateShortCodeResponse(
                    urlItem.ShortCode,
                    urlItem.OriginalUrl,
                    urlItem.CreatedAt
                );

                return Results.Created($"/api/shortcode/{urlItem.ShortCode}", response);
            })
            .WithName("CreateShortLink");
        }
    }
}

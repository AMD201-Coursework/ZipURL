using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Helpers;
using ZipURL.Services.ShorterURL.Models;


namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.CreateShortCode
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder group)
        {
            group.MapPost("/", async (CreateShortCodeRequest req, URLAppDbContext db,
                [FromServices] IHttpClientFactory httpClientFactory) =>
            {
                // 1. Validation cú pháp cơ bản
                if (string.IsNullOrWhiteSpace(req.TargetUrl) || !Uri.TryCreate(req.TargetUrl, UriKind.Absolute, out var uriResult))
                {
                    return Results.BadRequest(new { message = "Invalid URL format." });
                }

                // 2. Gửi HEAD Request để check xem link có thực sự tồn tại không
                try
                {
                    var client = httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(5); // Chỉ đợi tối đa 5 giây

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

                // 1. Validation
                var existingLink = await db.URLItems
                    .FirstOrDefaultAsync(x => x.OriginalUrl == req.TargetUrl && x.UserId == req.UserId);

                if (existingLink != null)
                {
                    return Results.Conflict(new
                    {
                        message = "You have already shortened this URL.",
                        shortCode = existingLink.ShortCode
                    });
                }


                // 2. Create first Entity with null ShortCode
                var urlItem = new URLItem
                {
                    OriginalUrl = req.TargetUrl,
                    UserId = req.UserId,
                    CreatedAt = DateTime.UtcNow,
                    ClickCount = 0,
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

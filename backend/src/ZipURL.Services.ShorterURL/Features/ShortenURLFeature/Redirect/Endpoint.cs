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
                // 1. KIỂM TRA REDIS (Bọc try-catch để nếu Redis lỗi vẫn chạy tiếp được)
                try 
                {
                    string cachedUrl = await cache.GetStringAsync(shortCode);
                    if (!string.IsNullOrEmpty(cachedUrl))
                    {
                        if (cachedUrl == "DISABLED")
                            return Results.NotFound(new { message = "Link này đã bị tắt." });

                        return Results.Redirect(cachedUrl);
                    }
                }
                catch { /* Bỏ qua lỗi Redis, tiếp tục kiểm tra DB */ }

                // 2. GIẢI MÃ SHORTCODE
                var id = HashidHelper.Decode(shortCode);
                if (id <= 0)
                    return Results.NotFound(new { message = "Mã rút gọn không hợp lệ hoặc không tồn tại." });

                // 3. TRUY VẤN DATABASE
                var urlItem = await db.URLItems.FindAsync(id);
                if (urlItem == null)
                    return Results.NotFound(new { message = "Đường dẫn không tồn tại trên hệ thống." });

                // 4. KIỂM TRA IsActive
                if (!urlItem.IsActive)
                {
                    // Cache trạng thái disabled 1 giờ để tránh DB query liên tục
                    var disabledOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));
                    await cache.SetStringAsync(shortCode, "DISABLED", disabledOptions);

                    return Results.NotFound(new { message = "Link này đã bị tắt." });
                }

                // 5. CẬP NHẬT CACHE
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(3));
                await cache.SetStringAsync(shortCode, urlItem.OriginalUrl, cacheOptions);

                // 6. ĐIỀU HƯỚNG
                return Results.Redirect(urlItem.OriginalUrl);
            })
            .WithName("RedirectToOriginal");
        }
    }
}
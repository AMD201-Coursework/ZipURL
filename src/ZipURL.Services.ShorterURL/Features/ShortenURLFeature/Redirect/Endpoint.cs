using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using ZipURL.Services.ShorterURL.Data;
using ZipURL.Services.ShorterURL.Helpers;

namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.Redirect
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            // Route gốc bắt mã shortCode (Ví dụ: domain.com/jR8pkL)
            app.MapGet("/{shortCode}", async (
                string shortCode,
                URLAppDbContext db,
                IDistributedCache cache) =>
            {
                // --- 1. KIỂM TRA REDIS (CACHE ASIDE) ---
                // Thử lấy URL gốc từ Redis trước để đạt tốc độ cao nhất
                string cachedUrl = await cache.GetStringAsync(shortCode);

                if (!string.IsNullOrEmpty(cachedUrl))
                {
                    // Nếu tìm thấy trong Cache, điều hướng ngay (302 Found)
                    return Results.Redirect(cachedUrl);
                }

                // --- 2. GIẢI MÃ SHORTCODE (HASHIDS) ---
                // Chuyển chuỗi "jR8pkL" ngược lại thành số ID (ví dụ: 123)
                var id = HashidHelper.Decode(shortCode);

                // Nếu mã không hợp lệ (Hashid trả về 0), báo lỗi 404
                if (id <= 0)
                {
                    return Results.NotFound(new { message = "Mã rút gọn không hợp lệ hoặc không tồn tại." });
                }

                // --- 3. TRUY VẤN DATABASE (NẾU CACHE MISS) ---
                // Tìm link trong DB theo ID (Truy vấn theo Primary Key cực nhanh)
                var urlItem = await db.URLItems.FindAsync(id);

                if (urlItem == null)
                {
                    return Results.NotFound(new { message = "Đường dẫn không tồn tại trên hệ thống." });
                }

                // --- 4. CẬP NHẬT CACHE VÀ THỐNG KÊ ---
                // Lưu vào Redis để các lượt click sau không cần vào DB nữa
                // Thiết lập thời gian hết hạn là 24 giờ
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(3));

                await cache.SetStringAsync(shortCode, urlItem.OriginalUrl, cacheOptions);

                // --- 5. ĐIỀU HƯỚNG ---
                // Trả về lệnh chuyển hướng cho trình duyệt
                return Results.Redirect(urlItem.OriginalUrl);
            })
            .WithName("RedirectToOriginal"); // Đặt tên để có thể gọi lại từ code khác
        }
    }
}

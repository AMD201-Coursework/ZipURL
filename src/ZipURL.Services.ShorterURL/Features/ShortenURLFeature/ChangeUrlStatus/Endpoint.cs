using ZipURL.Services.ShorterURL.Data;

namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.ChangeUrlStatus
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder group)
        {
            // Đổi đường dẫn thành /status để rõ nghĩa là cập nhật trạng thái
            group.MapPatch("/{id}/status", async (int id, URLAppDbContext db) =>
            {
                var urlItem = await db.URLItems.FindAsync(id);

                if (urlItem is null)
                {
                    return Results.NotFound(new { message = "Not Found." });
                }

                // Logic vẫn là đảo trạng thái (Toggle) nhưng tên biến rõ ràng hơn
                bool trangThaiMoi = !urlItem.IsActive;
                urlItem.IsActive = trangThaiMoi;

                await db.SaveChangesAsync();

                string thongBao = trangThaiMoi ? "Url activated successfully" : "Url deactivated successfully";

                return Results.Ok(new
                {
                    id = urlItem.Id,
                    isActive = urlItem.IsActive,
                    message = thongBao
                });
            })
            .WithName("UpdateUrlActiveStatus");
        }
    }
}

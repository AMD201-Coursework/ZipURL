using ZipURL.Services.ShorterURL.Data;

namespace ZipURL.Services.ShorterURL.Features.ShortenURLFeature.ChangeUrlStatus
{
    public class Endpoint
    {
        public static void Map(IEndpointRouteBuilder group)
        {
            // /status
            group.MapPatch("/{id}/status", async (int id, URLAppDbContext db) =>
            {
                var urlItem = await db.URLItems.FindAsync(id);

                if (urlItem is null)
                {
                    return Results.NotFound(new { message = "Not Found." });
                }

                // Logic 
                bool newStatus = !urlItem.IsActive;
                urlItem.IsActive = newStatus;

                await db.SaveChangesAsync();

                string notice = newStatus ? "Url activated successfully" : "Url deactivated successfully";

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

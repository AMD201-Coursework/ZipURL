using Microsoft.EntityFrameworkCore;
using ZipURL.Services.ShorterURL.Models;

namespace ZipURL.Services.ShorterURL.Data
{
    public class URLAppDbContext : DbContext
    {
        public URLAppDbContext(DbContextOptions<URLAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<URLItem> URLItems { get; set; }
    }
}

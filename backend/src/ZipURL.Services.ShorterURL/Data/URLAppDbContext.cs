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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<URLItem>(entity =>
            {
                // Index by user
                entity.HasIndex(e => e.UserId);

                // Index by short code for fast lookup, and unique constraint
                entity.HasIndex(u => u.ShortCode)
                      .IsUnique();

                // Short code is required but can be null (auto-generated if null)
                // Max length 10 for custom short codes
                entity.Property(u => u.ShortCode)
                      .IsRequired(false)
                      .HasMaxLength(10);
            });
        }
    }
}

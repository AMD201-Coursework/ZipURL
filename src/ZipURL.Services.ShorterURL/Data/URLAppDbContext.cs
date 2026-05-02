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

                entity.HasIndex(e => e.UserId);

                entity.HasIndex(u => u.ShortCode)
                      .IsUnique();

                entity.Property(u => u.ShortCode)
                      .IsRequired(false)
                      .HasMaxLength(10);
            });
        }
    }
}

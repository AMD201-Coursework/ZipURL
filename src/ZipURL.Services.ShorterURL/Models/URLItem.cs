using System.ComponentModel.DataAnnotations;

namespace ZipURL.Services.ShorterURL.Models
{
    public class URLItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string ShortCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ClickCount { get; set; } = 0;

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
    }
}

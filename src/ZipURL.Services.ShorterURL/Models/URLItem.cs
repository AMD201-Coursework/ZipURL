using System.ComponentModel.DataAnnotations;

namespace ZipURL.Services.ShorterURL.Models
{
    public class URLItem
    {
        [Key]
        public int Id { get; set; }

        [Required]  
        [Url]
        public string OriginalUrl { get; set; } = string.Empty;

        [StringLength(10)]
        public string? ShortCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        //public int ClickCount { get; set; } = 0;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Models
{
    public class Blog
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [StringLength(500)]
        public string? Quote { get; set; }
        
        public int AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

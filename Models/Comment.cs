using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelServiceAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int BlogId { get; set; }

        // SỬA: Cho phép UserId null
        public int? UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        // Thêm fields cho guest comments
        [StringLength(100)]
        public string? GuestName { get; set; }

        [StringLength(100)]
        public string? GuestEmail { get; set; }

        // Navigation properties
        [ForeignKey("BlogId")]
        public virtual Blog? Blog { get; set; }

        public virtual User? User { get; set; }
    }
}

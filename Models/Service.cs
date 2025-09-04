using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelServiceAPI.Models
{
    public class Service  // ĐỔI TỪ HotelService THÀNH Service
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [StringLength(100)]
        public string? Icon { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public int CreatedBy { get; set; }
        // public virtual User CreatedByUser { get; set; } = null!; // Tạm thời comment
    }
}

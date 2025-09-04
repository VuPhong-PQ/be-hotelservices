using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Models.DTOs
{
    public class ServiceCreateDto
    {
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
        public decimal Price { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class ServiceUpdateDto
    {
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
        public decimal Price { get; set; }
        
        [StringLength(50)]
        public string? Category { get; set; }
        
        public bool IsActive { get; set; }
    }

    public class ServiceExportDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Icon { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ServiceImportDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Icon { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

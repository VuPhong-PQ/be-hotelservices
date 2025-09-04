using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(20)]
        public string Role { get; set; } = "User"; // QUAN TRỌNG: Property Role
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Computed property
        public string FullName => $"{FirstName} {LastName}";
        
        // Navigation properties
        public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>(); // Sẽ cấu hình lại trong DbContext
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        // public virtual ICollection<Service> CreatedServices { get; set; } = new List<Service>(); // Tạm thời comment
    }
}

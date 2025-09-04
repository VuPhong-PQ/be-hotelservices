
using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        public DateTime BookingDate { get; set; }
        
        [Required]
        public DateTime ServiceDate { get; set; }
        
        public int NumberOfPeople { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = "Pending";
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Customer info fields
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }
        // Payment fields
        [MaxLength(32)]
        public string PaymentMethod { get; set; } = "Cash"; // Cash, CreditCard, Momo, Zalo, BankTransfer, Paypal, PayToRoom

        [MaxLength(16)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Unpaid, Paid, ...

        // Navigation properties
        public User? User { get; set; }
        public Service? Service { get; set; }
    }
}

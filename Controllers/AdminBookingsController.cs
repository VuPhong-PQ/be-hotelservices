using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelServiceAPI.Data;
using HotelServiceAPI.Models;

namespace HotelServiceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/bookings")]
    public class AdminBookingsController : ControllerBase
    {
        private readonly HotelDbContext _context;
        public AdminBookingsController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/bookings?search=...&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    (b.User != null && (b.User.FullName.Contains(search) || b.User.FirstName.Contains(search) || b.User.Email.Contains(search))) ||
                    (b.Service != null && b.Service.Name.Contains(search)) ||
                    b.Status.Contains(search) ||
                    b.PaymentMethod.Contains(search) ||
                    b.PaymentStatus.Contains(search)
                );
            }
            var total = await query.CountAsync();
            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(new { total, bookings });
        }

        // POST: api/admin/bookings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Booking booking)
        {
            if (booking == null)
                return BadRequest("Booking data is required");
            // Validate các trường bắt buộc
            if (booking.ServiceId == 0 || booking.BookingDate == default || booking.ServiceDate == default)
                return BadRequest("Missing required fields");
            booking.CreatedAt = DateTime.UtcNow;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }

        // PUT: api/admin/bookings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Booking booking)
        {
            if (booking == null || id != booking.Id)
                return BadRequest("Invalid booking data");
            var existing = await _context.Bookings.FindAsync(id);
            if (existing == null)
                return NotFound();
            // Cập nhật các trường cần thiết
            existing.UserId = booking.UserId;
            existing.ServiceId = booking.ServiceId;
            existing.BookingDate = booking.BookingDate;
            existing.ServiceDate = booking.ServiceDate;
            existing.NumberOfPeople = booking.NumberOfPeople;
            existing.Status = booking.Status;
            existing.PaymentMethod = booking.PaymentMethod;
            existing.PaymentStatus = booking.PaymentStatus;
            existing.Notes = booking.Notes;
            // Cập nhật các trường thông tin khách hàng
            existing.FirstName = booking.FirstName;
            existing.LastName = booking.LastName;
            existing.Email = booking.Email;
            existing.Phone = booking.Phone;
            existing.Address = booking.Address;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE: api/admin/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}

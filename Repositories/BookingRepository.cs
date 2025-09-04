
using Microsoft.EntityFrameworkCore;
using HotelServiceAPI.Data;
using HotelServiceAPI.Models;

namespace HotelServiceAPI.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelDbContext _context;

        public BookingRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Service)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            var existing = await _context.Bookings.FindAsync(booking.Id);
            if (existing == null) return booking;
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
            return existing;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


using HotelServiceAPI.Models;

namespace HotelServiceAPI.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
    }
}

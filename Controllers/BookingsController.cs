
using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Models;
using HotelServiceAPI.Repositories;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUser(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            var createdBooking = await _bookingRepository.CreateBookingAsync(booking);
            return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            var existingBooking = await _bookingRepository.GetBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            await _bookingRepository.UpdateBookingAsync(booking);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingRepository.DeleteBookingAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}

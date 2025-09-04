using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Data;

namespace HotelServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactMessagesController : ControllerBase
    {
        private readonly HotelDbContext _context;
        public ContactMessagesController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostContactMessage([FromBody] ContactMessage message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            message.CreatedAt = DateTime.UtcNow;
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}

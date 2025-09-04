using HotelServiceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/contactmessages")]
    public class AdminContactMessagesController : ControllerBase
    {
        private readonly HotelDbContext _context;
        public AdminContactMessagesController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/contactmessages?search=...&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.ContactMessages.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.Name.Contains(search) || m.Email.Contains(search) || m.Message.Contains(search));
            }
            var total = await query.CountAsync();
            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(new { total, messages });
        }

        // DELETE: api/admin/contactmessages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var msg = await _context.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();
            _context.ContactMessages.Remove(msg);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}

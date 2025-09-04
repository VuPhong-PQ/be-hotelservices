using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace HotelServiceAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HotelDbContext _context;

        public UserRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Blogs)
                .Include(u => u.Comments)
                // .Include(u => u.CreatedServices) // Tạm thời comment
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Blogs)
                .Include(u => u.Comments)
                // .Include(u => u.CreatedServices) // Tạm thời comment
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Hash password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }
            return null;
        }
    }
}

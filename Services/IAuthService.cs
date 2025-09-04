using HotelServiceAPI.Models;

namespace HotelServiceAPI.Services
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User> RegisterUserAsync(User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
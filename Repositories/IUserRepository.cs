using HotelServiceAPI.Models;

namespace HotelServiceAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetByIdAsync(int id); // Thêm method này
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(string email);
        Task<User?> ValidateUserAsync(string email, string password);
    }
}
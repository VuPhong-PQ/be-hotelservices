using Microsoft.EntityFrameworkCore;
using HotelServiceAPI.Models;
using HotelServiceAPI.Models.DTOs;
using HotelServiceAPI.Data;

namespace HotelServiceAPI.Services
{
    public interface ISqlServerService
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<Service?> GetServiceByIdAsync(int id);
        Task<Service> CreateServiceAsync(Service service);
        Task<Service?> UpdateServiceAsync(int id, Service service);
        Task<bool> DeleteServiceAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<Service>> GetAllServicesForAdminAsync();
        Task<IEnumerable<ServiceExportDto>> GetAllServicesForExportAsync();
        Task<bool> DeleteAllServicesAsync();
    }

    public class SqlServerService : ISqlServerService
    {
        private readonly HotelDbContext _context;

        public SqlServerService(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetAllServicesForAdminAsync()
        {
            return await _context.Services
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            service.CreatedAt = DateTime.UtcNow;
            service.UpdatedAt = DateTime.UtcNow;
            
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            
            return service;
        }

        public async Task<Service?> UpdateServiceAsync(int id, Service service)
        {
            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
            {
                return null;
            }

            existingService.Name = service.Name;
            existingService.Description = service.Description;
            existingService.ImageUrl = service.ImageUrl;
            existingService.Icon = service.Icon;
            existingService.Price = service.Price;
            existingService.Category = service.Category;
            existingService.IsActive = service.IsActive;
            existingService.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingService;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return false;
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return user;
        }

        public async Task<IEnumerable<ServiceExportDto>> GetAllServicesForExportAsync()
        {
            return await _context.Services
                .Select(s => new ServiceExportDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    Icon = s.Icon,
                    Price = s.Price,
                    Category = s.Category,
                    IsActive = s.IsActive,
                    CreatedByName = "Admin", // Hardcode vì đã comment navigation property
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAllServicesAsync()
        {
            try
            {
                var allServices = await _context.Services.ToListAsync();
                if (allServices.Any())
                {
                    _context.Services.RemoveRange(allServices);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false; // Không có services để xóa
            }
            catch
            {
                return false;
            }
        }
    }
}

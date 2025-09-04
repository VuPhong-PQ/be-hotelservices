using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using HotelServiceAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly HotelDbContext _context;

        public ServiceRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _context.Services
                // .Include(s => s.CreatedByUser) // Tạm thời comment để tránh lỗi FK
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _context.Services
                // .Include(s => s.CreatedByUser) // Tạm thời comment để tránh lỗi FK
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category)
        {
            return await _context.Services
                // .Include(s => s.CreatedByUser) // Tạm thời comment để tránh lỗi FK
                .Where(s => s.IsActive && s.Category == category)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _context.Services
                // .Include(s => s.CreatedByUser) // Tạm thời comment để tránh lỗi FK
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Service> CreateAsync(Service service)
        {
            service.CreatedAt = DateTime.UtcNow;
            service.UpdatedAt = DateTime.UtcNow;
            
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            
            // Return the service without navigation properties to avoid the FK issue for now
            return service;
        }

        public async Task<Service> UpdateAsync(Service service)
        {
            service.UpdatedAt = DateTime.UtcNow;
            
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
            
            return await GetByIdAsync(service.Id) ?? service;
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _context.Services
                .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Category))
                .Select(s => s.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        // For admin - get all services including inactive ones
        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services
                // .Include(s => s.CreatedByUser) // Tạm thời comment để tránh lỗi FK
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        // For Excel export with detailed information
        public async Task<IEnumerable<ServiceExportDto>> GetAllServicesForExportAsync()
        {
            return await _context.Services
                // .Include(s => s.CreatedByUser) // Tạm thời comment để tránh lỗi FK
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
                    CreatedByName = "Admin", // s.CreatedByUser.FullName, // Tạm thời hardcode
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
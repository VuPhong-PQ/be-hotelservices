using HotelServiceAPI.Models;
using HotelServiceAPI.Models.DTOs;

namespace HotelServiceAPI.Repositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<IEnumerable<Service>> GetAllServicesAsync(); // For admin - includes inactive
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(string category);
        Task<Service?> GetByIdAsync(int id);
        Task<Service> CreateAsync(Service service);
        Task<Service> UpdateAsync(Service service);
        Task DeleteAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<ServiceExportDto>> GetAllServicesForExportAsync(); // For Excel export
    }
}

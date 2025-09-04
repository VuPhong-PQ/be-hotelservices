using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelServiceAPI.Repositories;
using HotelServiceAPI.Models;
using HotelServiceAPI.Models.DTOs;
using HotelServiceAPI.Services;
using System.Security.Claims;

namespace HotelServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ISqlServerService _sqlServerService;
        private readonly IExcelService _excelService;

        public ServicesController(ISqlServerService sqlServerService, IExcelService excelService)
        {
            _sqlServerService = sqlServerService;
            _excelService = excelService;
        }

        // GET: api/services
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            try
            {
                var services = await _sqlServerService.GetAllServicesAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi EF Core: " + ex.Message });
            }
        }

        // GET: api/services/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            try
            {
                var service = await _sqlServerService.GetServiceByIdAsync(id);
                if (service == null)
                {
                    return NotFound(new { message = "Service không tồn tại" });
                }
                return Ok(service);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi EF Core: " + ex.Message });
            }
        }

        // ============= ADMIN ENDPOINTS =============

        // GET: api/services/admin/all - Chỉ admin xem được tất cả services (kể cả inactive)
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllServicesForAdmin()
        {
            try
            {
                var services = await _sqlServerService.GetAllServicesForAdminAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi EF Core: " + ex.Message });
            }
        }

        // POST: api/services - Tạo service mới (chỉ admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateService([FromBody] ServiceCreateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Lấy user ID từ JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var service = new Service
                {
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    Icon = request.Icon,
                    Price = request.Price,
                    Category = request.Category,
                    IsActive = request.IsActive,
                    CreatedBy = userId, // Sử dụng user ID từ token
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdService = await _sqlServerService.CreateServiceAsync(service);
                return CreatedAtAction(nameof(GetService), new { id = createdService.Id }, createdService);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi EF Core: " + ex.Message });
            }
        }

        // PUT: api/services/{id} - Cập nhật service (chỉ admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceUpdateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingService = await _sqlServerService.GetServiceByIdAsync(id);
                if (existingService == null)
                {
                    return NotFound(new { message = "Service không tồn tại" });
                }

                // Tạo service object mới với dữ liệu cập nhật
                var serviceToUpdate = new Service
                {
                    Name = request.Name,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    Icon = request.Icon,
                    Price = request.Price,
                    Category = request.Category,
                    IsActive = request.IsActive,
                    CreatedBy = existingService.CreatedBy // Giữ nguyên CreatedBy
                };

                var updatedService = await _sqlServerService.UpdateServiceAsync(id, serviceToUpdate);
                return Ok(updatedService);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi EF Core: " + ex.Message });
            }
        }

        // DELETE: api/services/{id} - Xóa service (chỉ admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var deleted = await _sqlServerService.DeleteServiceAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "Service không tồn tại" });
                }

                return Ok(new { message = "Xóa service thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi EF Core: " + ex.Message });
            }
        }

        // GET: api/services/export - Export services to Excel (chỉ admin)
        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportServicesToExcel()
        {
            try
            {
                var services = await _sqlServerService.GetAllServicesForExportAsync();
                var excelData = await _excelService.ExportServicesToExcelAsync(services);

                var fileName = $"Services_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi EF Core khi export: {ex.Message}" });
            }
        }

        // POST: api/services/import - Import services from Excel (chỉ admin)
        [HttpPost("import")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportServicesFromExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Vui lòng chọn file Excel" });
                }

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    return BadRequest(new { message = "File phải có định dạng Excel (.xlsx hoặc .xls)" });
                }

                // Get current user ID from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(new { message = "Không thể xác định người dùng" });
                }

                using var stream = file.OpenReadStream();
                var importedServices = await _excelService.ImportServicesFromExcelAsync(stream);

                var successCount = 0;
                var errors = new List<string>();

                foreach (var serviceDto in importedServices)
                {
                    try
                    {
                        var service = new Service
                        {
                            Name = serviceDto.Name,
                            Description = serviceDto.Description,
                            ImageUrl = serviceDto.ImageUrl,
                            Icon = serviceDto.Icon,
                            Price = serviceDto.Price,
                            Category = serviceDto.Category,
                            IsActive = serviceDto.IsActive,
                            CreatedBy = userId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _sqlServerService.CreateServiceAsync(service);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lỗi khi import service '{serviceDto.Name}': {ex.Message}");
                    }
                }

                return Ok(new 
                { 
                    message = $"Import thành công {successCount} services",
                    successCount,
                    errorCount = errors.Count,
                    errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/services/all - Xóa tất cả services (chỉ admin)
        [HttpDelete("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAllServices()
        {
            try
            {
                var result = await _sqlServerService.DeleteAllServicesAsync();
                if (result)
                {
                    return Ok(new { message = "Đã xóa tất cả services thành công" });
                }
                else
                {
                    return NotFound(new { message = "Không có services nào để xóa" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi EF Core khi xóa tất cả services: {ex.Message}" });
            }
        }
    }
}

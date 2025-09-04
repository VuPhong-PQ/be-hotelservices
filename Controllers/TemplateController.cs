using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelServiceAPI.Services;

namespace HotelServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly IExcelService _excelService;

        public TemplateController(IExcelService excelService)
        {
            _excelService = excelService;
        }

        // GET: api/template/services-import - Download template Excel để import services
        [HttpGet("services-import")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadServicesImportTemplate()
        {
            try
            {
                // Tạo sample data để làm template
                var sampleServices = new List<HotelServiceAPI.Models.DTOs.ServiceExportDto>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Dọn phòng",
                        Description = "Dịch vụ dọn dẹp phòng hàng ngày",
                        ImageUrl = "https://example.com/cleaning.jpg",
                        Icon = "ri-brush-line",
                        Price = 100000,
                        Category = "Phòng",
                        IsActive = true,
                        CreatedByName = "Admin",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Spa thư giãn",
                        Description = "Dịch vụ spa cao cấp với đội ngũ chuyên nghiệp",
                        ImageUrl = "https://example.com/spa.jpg",
                        Icon = "ri-heart-line",
                        Price = 500000,
                        Category = "Spa",
                        IsActive = true,
                        CreatedByName = "Admin",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };

                var excelData = await _excelService.ExportServicesToExcelAsync(sampleServices);
                var fileName = "Services_Import_Template.xlsx";
                
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

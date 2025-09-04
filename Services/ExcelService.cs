using OfficeOpenXml;
using HotelServiceAPI.Models;
using HotelServiceAPI.Models.DTOs;

namespace HotelServiceAPI.Services
{
    public interface IExcelService
    {
        Task<byte[]> ExportServicesToExcelAsync(IEnumerable<ServiceExportDto> services);
        Task<IEnumerable<ServiceImportDto>> ImportServicesFromExcelAsync(Stream excelFile);
    }

    public class ExcelService : IExcelService
    {
        public async Task<byte[]> ExportServicesToExcelAsync(IEnumerable<ServiceExportDto> services)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Services");

            // Add headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Tên dịch vụ";
            worksheet.Cells[1, 3].Value = "Mô tả";
            worksheet.Cells[1, 4].Value = "Hình ảnh URL";
            worksheet.Cells[1, 5].Value = "Icon";
            worksheet.Cells[1, 6].Value = "Giá";
            worksheet.Cells[1, 7].Value = "Danh mục";
            worksheet.Cells[1, 8].Value = "Trạng thái";
            worksheet.Cells[1, 9].Value = "Người tạo";
            worksheet.Cells[1, 10].Value = "Ngày tạo";
            worksheet.Cells[1, 11].Value = "Ngày cập nhật";

            // Style headers
            using (var range = worksheet.Cells[1, 1, 1, 11])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            // Add data
            int row = 2;
            foreach (var service in services)
            {
                worksheet.Cells[row, 1].Value = service.Id;
                worksheet.Cells[row, 2].Value = service.Name;
                worksheet.Cells[row, 3].Value = service.Description;
                worksheet.Cells[row, 4].Value = service.ImageUrl;
                worksheet.Cells[row, 5].Value = service.Icon;
                worksheet.Cells[row, 6].Value = service.Price;
                worksheet.Cells[row, 7].Value = service.Category;
                worksheet.Cells[row, 8].Value = service.IsActive ? "Hoạt động" : "Không hoạt động";
                worksheet.Cells[row, 9].Value = service.CreatedByName;
                worksheet.Cells[row, 10].Value = service.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 11].Value = service.UpdatedAt.ToString("dd/MM/yyyy HH:mm");
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();

            // Add borders to all cells with data
            using (var range = worksheet.Cells[1, 1, row - 1, 11])
            {
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            }

            return await package.GetAsByteArrayAsync();
        }

        public Task<IEnumerable<ServiceImportDto>> ImportServicesFromExcelAsync(Stream excelFile)
        {
            var services = new List<ServiceImportDto>();

            using var package = new ExcelPackage(excelFile);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                throw new ArgumentException("Excel file không có worksheet nào");

            // Skip header row, start from row 2
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                try
                {
                    var service = new ServiceImportDto
                    {
                        Name = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "",
                        Description = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "",
                        ImageUrl = worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                        Icon = worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                        Price = decimal.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out var price) ? price : 0,
                        Category = worksheet.Cells[row, 7].Value?.ToString()?.Trim(),
                        IsActive = worksheet.Cells[row, 8].Value?.ToString()?.Trim()?.ToLower() != "không hoạt động"
                    };

                    // Validate required fields
                    if (!string.IsNullOrEmpty(service.Name) && !string.IsNullOrEmpty(service.Description))
                    {
                        services.Add(service);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other rows
                    Console.WriteLine($"Error processing row {row}: {ex.Message}");
                }
            }

            return Task.FromResult<IEnumerable<ServiceImportDto>>(services);
        }
    }
}

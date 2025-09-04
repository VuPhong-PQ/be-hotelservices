using Microsoft.AspNetCore.Mvc;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public UploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được chọn" });
                }

                // Check file size
                if (file.Length > _maxFileSize)
                {
                    return BadRequest(new { message = "File quá lớn. Kích thước tối đa là 5MB" });
                }

                // Check file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, bmp, webp)" });
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "images");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the URL path
                var fileUrl = $"/uploads/images/{fileName}";

                return Ok(new 
                { 
                    message = "Upload thành công",
                    fileName = fileName,
                    url = fileUrl,
                    fullUrl = $"{Request.Scheme}://{Request.Host}{fileUrl}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        [HttpDelete("image/{fileName}")]
        public IActionResult DeleteImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", "images", fileName);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { message = "Xóa file thành công" });
                }
                
                return NotFound(new { message = "Không tìm thấy file" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }
    }
}

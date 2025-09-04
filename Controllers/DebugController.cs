using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public DebugController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet("database-status")]
        public async Task<IActionResult> GetDatabaseStatus()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                var blogCount = await _context.Blogs.CountAsync();
                var serviceCount = await _context.Services.CountAsync();

                return Ok(new
                {
                    DatabaseConnected = true,
                    UserCount = userCount,
                    BlogCount = blogCount,
                    ServiceCount = serviceCount,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    DatabaseConnected = false,
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("users-roles")]
        public async Task<IActionResult> GetUsersAndRoles()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.Role,
                        u.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    Success = true,
                    Users = users,
                    TotalUsers = users.Count,
                    AdminUsers = users.Count(u => u.Role == "Admin"),
                    RegularUsers = users.Count(u => u.Role == "User"),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
                
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users.Select(u => new 
                { 
                    u.Id, 
                    u.Email, 
                    u.FirstName, 
                    u.LastName, 
                    u.Role,
                    PasswordHash = u.Password // For debugging only
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("make-admin/{email}")]
        public async Task<IActionResult> MakeUserAdmin(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return NotFound(new { message = $"User với email {email} không tồn tại" });
                }

                user.Role = "Admin";
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = $"Đã cập nhật user {user.FirstName} {user.LastName} ({email}) thành Admin",
                    user = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return NotFound(new { message = $"User với email {request.Email} không tồn tại" });
                }

                // Hash password using BCrypt (same as UserRepository)
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = $"Đã cập nhật mật khẩu cho user {user.FirstName} {user.LastName} ({request.Email})",
                    user = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("create-sample-services")]
        public async Task<IActionResult> CreateSampleServices()
        {
            try
            {
                // Xóa tất cả services cũ
                var existingServices = await _context.Services.ToListAsync();
                _context.Services.RemoveRange(existingServices);
                await _context.SaveChangesAsync();

                // Tạo sample services đầy đủ
                var sampleServices = new List<Service>
                {
                    new Service
                    {
                        Name = "Spa & Wellness Center",
                        Description = "Thư giãn và tái tạo năng lượng với các liệu pháp spa cao cấp, massage trị liệu và dịch vụ chăm sóc sức khỏe toàn diện",
                        ImageUrl = "https://images.unsplash.com/photo-1544161515-4ab6ce6db874?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                        Icon = "fa-spa",
                        Price = 1500000.00m,
                        Category = "Spa",
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name = "Nhà Hàng Cao Cấp",
                        Description = "Trải nghiệm ẩm thực đẳng cấp với các món ăn được chế biến bởi đầu bếp Michelin, phục vụ trong không gian sang trọng",
                        ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                        Icon = "fa-utensils",
                        Price = 2000000.00m,
                        Category = "Ẩm thực",
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name = "Gym & Fitness Center",
                        Description = "Phòng gym hiện đại với đầy đủ trang thiết bị tập luyện cao cấp, huấn luyện viên cá nhân và các lớp group fitness",
                        ImageUrl = "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                        Icon = "fa-dumbbell",
                        Price = 800000.00m,
                        Category = "Thể thao",
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name = "Dịch Vụ Hồ Bơi VIP",
                        Description = "Hồ bơi riêng tư với view panoramic, dịch vụ poolside service và khu vực nghỉ dưỡng độc quyền",
                        ImageUrl = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                        Icon = "fa-swimmer",
                        Price = 1200000.00m,
                        Category = "Giải trí",
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name = "Dịch Vụ Concierge 24/7",
                        Description = "Dịch vụ quản gia cá nhân 24/7, hỗ trợ đặt tour, mua sắm, và mọi nhu cầu trong thời gian lưu trú",
                        ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                        Icon = "fa-concierge-bell",
                        Price = 500000.00m,
                        Category = "Dịch vụ",
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name = "Bar Rooftop Exclusive",
                        Description = "Bar cao tầng với view thành phố tuyệt đẹp, cocktails signature và không gian thư giãn cao cấp",
                        ImageUrl = "https://images.unsplash.com/photo-1569949381669-ecf31ae8e613?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                        Icon = "fa-cocktail",
                        Price = 900000.00m,
                        Category = "Giải trí",
                        IsActive = true,
                        CreatedBy = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                _context.Services.AddRange(sampleServices);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = $"Đã tạo {sampleServices.Count} sample services thành công",
                    services = sampleServices.Select(s => new { s.Id, s.Name, s.Price, s.Category }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class ChangePasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
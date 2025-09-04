
using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Models;
using HotelServiceAPI.Repositories;
using HotelServiceAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public UsersController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userRepository.UserExistsAsync(request.Email))
            {
                return BadRequest(new { message = "Email đã được sử dụng" });
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                Phone = request.Phone
            };

            try
            {
                var createdUser = await _userRepository.CreateUserAsync(user);
                return Ok(new
                {
                    message = "Đăng ký thành công",
                    user = createdUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.ValidateUserAsync(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
            }

            // Generate JWT token
            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new
            {
                message = "Đăng nhập thành công",
                user = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    FullName = user.FullName,
                    user.Email,
                    user.Phone,
                    user.Role
                },
                token = token
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userRepository.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userRepository.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ là bắt buộc")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = string.Empty;
    }
}

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HotelServiceAPI.Models;
using HotelServiceAPI.Repositories;

namespace HotelServiceAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("userId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null && VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            user.Password = HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.Role = user.Role ?? "User";
            
            return await _userRepository.CreateUserAsync(user);
        }

        public string HashPassword(string password)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    byte[] hashBytes = new byte[48];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 32);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                            return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
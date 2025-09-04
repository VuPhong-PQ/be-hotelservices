using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelServiceAPI.Data;
using HotelServiceAPI.Repositories;
using HotelServiceAPI.Services;
using HotelServiceAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Đăng ký HotelDbContext cho migration và API (chỉ dùng 1 context)
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ISqlServerService, SqlServerService>();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "default-secret-key"))
    };
});

// Authorization
builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000", 
                "http://127.0.0.1:3000",
                "http://localhost:3001",
                "http://127.0.0.1:3001",
                "http://localhost:3002",
                "http://127.0.0.1:3002"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true);
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Thêm middleware logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"Origin: {context.Request.Headers["Origin"]}");
    await next.Invoke();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});

// Static files
app.UseStaticFiles(); 

// SỬAA: Tạo thư mục uploads trước khi serve
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
var imagesPath = Path.Combine(uploadsPath, "images");

// Tạo thư mục nếu chưa tồn tại
Directory.CreateDirectory(uploadsPath);
Directory.CreateDirectory(imagesPath);

Console.WriteLine($"📁 Created uploads directory: {uploadsPath}");

// Serve uploads folder (sau khi đã tạo thư mục)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// Auto migrate và seed data (tạo user tự động mỗi khi run API)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
        Console.WriteLine("🔄 Migrating database...");
        context.Database.Migrate();
        // Seed admin user only
        context.SeedAdminUser();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database setup failed: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 API Starting...");
Console.WriteLine("📊 Available endpoints:");
Console.WriteLine("   GET  /api/blogs");
Console.WriteLine("   POST /api/blogs");
Console.WriteLine("   GET  /api/blogs/{id}");
Console.WriteLine("   PUT  /api/blogs/{id}");
Console.WriteLine("   DELETE /api/blogs/{id}");
Console.WriteLine("   GET  /api/blogs/my-blogs/{userId}");
Console.WriteLine("   POST /api/blogs/upload-image");
Console.WriteLine("   GET  /api/blogs/debug");
Console.WriteLine("   GET  /api/comments/blog/{blogId}");
Console.WriteLine("   GET  /api/comments/blog/{blogId}/with-permissions/{userId}");
Console.WriteLine("   POST /api/comments");
Console.WriteLine("   GET  /api/comments/{id}");
Console.WriteLine("   PUT  /api/comments/{id}");
Console.WriteLine("   DELETE /api/comments/{id}?userId={userId}");
Console.WriteLine("   DELETE /api/comments/batch");
Console.WriteLine("   GET  /api/comments/user/{userId}");
Console.WriteLine("   GET  /api/comments/debug");
Console.WriteLine("   GET  /swagger");

app.Run();
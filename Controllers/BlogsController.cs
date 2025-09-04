using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Models;
using HotelServiceAPI.Repositories;
using HotelServiceAPI.Data; // THÊM
using Microsoft.EntityFrameworkCore; // THÊM
using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;

        public BlogsController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // GET: api/blogs
        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            try
            {
                Console.WriteLine("🔄 GetBlogs called");
                
                var blogs = await _blogRepository.GetAllBlogsAsync();
                Console.WriteLine($"📊 Retrieved {blogs.Count()} blogs from repository");
                
                var blogDtos = blogs.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    b.ImageUrl,
                    b.Quote,
                    b.CreatedAt,
                    b.UpdatedAt,
                    Author = b.Author != null ? new 
                    { 
                        b.Author.Id, 
                        Name = b.Author.FullName, 
                        b.Author.Email 
                    } : null
                }).ToList();

                Console.WriteLine($"✅ Returning {blogDtos.Count} blog DTOs");
                return Ok(blogDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetBlogs Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Error retrieving blogs", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // POST: api/blogs
        [HttpPost]
        public async Task<ActionResult<Blog>> CreateBlog([FromBody] CreateBlogRequest request)
        {
            try
            {
                Console.WriteLine($"CreateBlog called with: {request.Title}");
                
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid");
                    return BadRequest(ModelState);
                }

                // THÊM: Validate AuthorId exists
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
                
                var userExists = await context.Users.AnyAsync(u => u.Id == request.AuthorId);
                if (!userExists)
                {
                    Console.WriteLine($"AuthorId {request.AuthorId} does not exist");
                    return BadRequest(new { message = $"User with ID {request.AuthorId} does not exist" });
                }

                var blog = new Blog
                {
                    Title = request.Title,
                    Content = request.Content,
                    ImageUrl = request.ImageUrl ?? "",
                    Quote = request.Quote ?? "",
                    AuthorId = request.AuthorId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                Console.WriteLine($"Creating blog: {blog.Title} for AuthorId: {blog.AuthorId}");
                
                var createdBlog = await _blogRepository.CreateBlogAsync(blog);
                
                Console.WriteLine($"Blog created successfully with ID: {createdBlog.Id}");
                
                // SỬA: Return Ok thay vì CreatedAtAction để tránh lỗi
                return Ok(new {
                    id = createdBlog.Id,
                    title = createdBlog.Title,
                    content = createdBlog.Content,
                    imageUrl = createdBlog.ImageUrl,
                    quote = createdBlog.Quote,
                    authorId = createdBlog.AuthorId,
                    createdAt = createdBlog.CreatedAt,
                    updatedAt = createdBlog.UpdatedAt,
                    message = "Blog created successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateBlog Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Error creating blog", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // GET: api/blogs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            try
            {
                var blog = await _blogRepository.GetBlogByIdAsync(id);
                if (blog == null)
                {
                    return NotFound();
                }
                
                var blogDto = new
                {
                    blog.Id,
                    blog.Title,
                    blog.Content,
                    blog.ImageUrl,
                    blog.Quote,
                    blog.CreatedAt,
                    blog.UpdatedAt,
                    Author = blog.Author != null ? new 
                    { 
                        blog.Author.Id, 
                        Name = blog.Author.FullName, 
                        blog.Author.Email 
                    } : null
                };
                
                return Ok(blogDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBlog Error: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving blog", error = ex.Message });
            }
        }

        // GET: api/blogs/my-blogs/{userId}
        [HttpGet("my-blogs/{userId}")]
        public async Task<IActionResult> GetMyBlogs(int userId)
        {
            try
            {
                Console.WriteLine($"GetMyBlogs called for userId: {userId}");
                
                var blogs = await _blogRepository.GetBlogsByAuthorIdAsync(userId);
                
                var blogDtos = blogs.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    b.ImageUrl,
                    b.Quote,
                    b.CreatedAt,
                    b.UpdatedAt,
                    Author = b.Author != null ? new 
                    { 
                        b.Author.Id, 
                        Name = b.Author.FullName, 
                        b.Author.Email 
                    } : null
                });

                Console.WriteLine($"Found {blogDtos.Count()} blogs for user {userId}");
                return Ok(blogDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetMyBlogs Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Error retrieving user blogs", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/blogs/author/{authorId} - cũng có thể dùng endpoint này
        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetBlogsByAuthor(int authorId)
        {
            try
            {
                Console.WriteLine($"GetBlogsByAuthor called for authorId: {authorId}");
                
                var blogs = await _blogRepository.GetBlogsByAuthorIdAsync(authorId);
                
                var blogDtos = blogs.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Content,
                    b.ImageUrl,
                    b.Quote,
                    b.CreatedAt,
                    b.UpdatedAt,
                    Author = b.Author != null ? new 
                    { 
                        b.Author.Id, 
                        Name = b.Author.FullName, 
                        b.Author.Email 
                    } : null
                });

                Console.WriteLine($"Found {blogDtos.Count()} blogs for author {authorId}");
                return Ok(blogDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBlogsByAuthor Error: {ex.Message}");
                return StatusCode(500, new { 
                    message = "Error retrieving author blogs", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/blogs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] UpdateBlogRequest request)
        {
            try
            {
                Console.WriteLine($"UpdateBlog called for blogId: {id}");
                
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid");
                    return BadRequest(ModelState);
                }

                // Kiểm tra blog có tồn tại không
                var existingBlog = await _blogRepository.GetBlogByIdAsync(id);
                if (existingBlog == null)
                {
                    Console.WriteLine($"Blog with ID {id} not found");
                    return NotFound(new { message = $"Blog with ID {id} not found" });
                }

                // Cập nhật thông tin blog
                existingBlog.Title = request.Title;
                existingBlog.Content = request.Content;
                existingBlog.ImageUrl = request.ImageUrl ?? existingBlog.ImageUrl;
                existingBlog.Quote = request.Quote ?? existingBlog.Quote;
                existingBlog.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"Updating blog: {existingBlog.Title}");
                
                var updatedBlog = await _blogRepository.UpdateBlogAsync(existingBlog);
                
                Console.WriteLine($"Blog updated successfully with ID: {updatedBlog.Id}");
                
                // Return updated blog data
                var blogDto = new
                {
                    updatedBlog.Id,
                    updatedBlog.Title,
                    updatedBlog.Content,
                    updatedBlog.ImageUrl,
                    updatedBlog.Quote,
                    updatedBlog.CreatedAt,
                    updatedBlog.UpdatedAt,
                    Author = updatedBlog.Author != null ? new 
                    { 
                        updatedBlog.Author.Id, 
                        Name = updatedBlog.Author.FullName, 
                        updatedBlog.Author.Email 
                    } : null,
                    message = "Blog updated successfully"
                };
                
                return Ok(blogDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateBlog Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Error updating blog", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // DELETE: api/blogs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            try
            {
                Console.WriteLine($"DeleteBlog called for blogId: {id}");
                
                // Kiểm tra blog có tồn tại không
                var existingBlog = await _blogRepository.GetBlogByIdAsync(id);
                if (existingBlog == null)
                {
                    Console.WriteLine($"Blog with ID {id} not found");
                    return NotFound(new { message = $"Blog with ID {id} not found" });
                }

                Console.WriteLine($"Deleting blog: {existingBlog.Title}");
                
                var success = await _blogRepository.DeleteBlogAsync(id);
                if (!success)
                {
                    return StatusCode(500, new { message = "Failed to delete blog" });
                }

                Console.WriteLine($"Blog deleted successfully with ID: {id}");
                
                return Ok(new { 
                    message = "Blog deleted successfully",
                    deletedBlogId = id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteBlog Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    message = "Error deleting blog", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/blogs/upload-image
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Only image files are allowed" });
                }

                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images");
                
                // Ensure directory exists
                Directory.CreateDirectory(uploadsPath);
                
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return URL
                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/images/{fileName}";
                
                Console.WriteLine($"Image uploaded: {imageUrl}");
                
                return Ok(new 
                { 
                    imageUrl = imageUrl,
                    fileName = fileName,
                    message = "Image uploaded successfully" 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload Error: {ex.Message}");
                return StatusCode(500, new { message = "Error uploading image", error = ex.Message });
            }
        }

        // GET: api/blogs/debug
        [HttpGet("debug")]
        public async Task<IActionResult> DebugBlogs()
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var blogCount = await context.Blogs.CountAsync();
                var userCount = await context.Users.CountAsync();
                
                var recentBlogs = await context.Blogs
                    .Include(b => b.Author)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(3)
                    .Select(b => new {
                        b.Id,
                        b.Title,
                        b.AuthorId,
                        AuthorName = b.Author != null ? b.Author.FullName : "No Author",
                        b.CreatedAt
                    })
                    .ToListAsync();
                
                return Ok(new {
                    blogCount,
                    userCount,
                    recentBlogs,
                    timestamp = DateTime.UtcNow,
                    message = "Debug info retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DebugBlogs Error: {ex.Message}");
                return StatusCode(500, new { 
                    message = "Debug failed", 
                    error = ex.Message 
                });
            }
        }
    }

    // DTO Classes
    public class CreateBlogRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string? ImageUrl { get; set; }
        
        public string? Quote { get; set; }
        
        [Required]
        public int AuthorId { get; set; }
    }

    public class UpdateBlogRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public string? ImageUrl { get; set; }
        
        public string? Quote { get; set; }
    }
}

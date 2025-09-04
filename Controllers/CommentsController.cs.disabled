using Microsoft.AspNetCore.Mvc;
using HotelServiceAPI.Models;
using HotelServiceAPI.Repositories;
using HotelServiceAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // ===== ROUTES CỤ THỂ - PHẢI ĐẶT TRƯỚC =====
        
        // GET: api/comments/blog/{blogId}/with-permissions/{userId}
        [HttpGet("blog/{blogId}/with-permissions/{userId}")]
        public async Task<IActionResult> GetCommentsWithPermissions(int blogId, int userId)
        {
            try
            {
                Console.WriteLine($"GetCommentsWithPermissions called for blogId: {blogId}, userId: {userId}");
                
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Kiểm tra blog
                var blog = await context.Blogs.FindAsync(blogId);
                if (blog == null)
                {
                    Console.WriteLine($"❌ Blog {blogId} not found");
                    return BadRequest(new { message = "Blog not found", requestedBlogId = blogId });
                }

                bool isBlogAuthor = blog.AuthorId == userId;
                Console.WriteLine($"✅ Blog found: {blog.Title}, Author: {blog.AuthorId}, isBlogAuthor: {isBlogAuthor}");
                
                var comments = await _commentRepository.GetCommentsByBlogIdAsync(blogId);
                Console.WriteLine($"Found {comments.Count()} comments for blog {blogId}");
                
                var commentDtos = comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.BlogId,
                    c.UserId,
                    c.GuestName,
                    c.GuestEmail,
                    User = c.User != null ? new 
                    { 
                        c.User.Id, 
                        Name = c.User.FullName, 
                        c.User.Email 
                    } : null,
                    AuthorName = c.User?.FullName ?? c.GuestName ?? "Anonymous",
                    IsGuest = c.UserId == null,
                    CanDelete = isBlogAuthor || c.UserId == userId,
                    DeleteReason = isBlogAuthor 
                        ? "You are the blog author" 
                        : (c.UserId == userId ? "This is your comment" : "No permission")
                });

                Console.WriteLine($"✅ Processed {commentDtos.Count()} comments with permissions");
                
                return Ok(new {
                    comments = commentDtos,
                    blogInfo = new {
                        blogId,
                        authorId = blog.AuthorId,
                        isBlogAuthor,
                        title = blog.Title
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetCommentsWithPermissions Error: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving comments with permissions", error = ex.Message });
            }
        }

        // GET: api/comments/debug-blog/{blogId}
        [HttpGet("debug-blog/{blogId}")]
        public async Task<IActionResult> DebugBlog(int blogId)
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var blog = await context.Blogs.FindAsync(blogId);
                var allBlogs = await context.Blogs.Select(b => new { b.Id, b.Title, b.AuthorId }).ToListAsync();
                var commentCount = await context.Comments.CountAsync(c => c.BlogId == blogId);
                
                return Ok(new {
                    requestedBlogId = blogId,
                    blogExists = blog != null,
                    blog = blog != null ? new { blog.Id, blog.Title, blog.AuthorId } : null,
                    allBlogs,
                    commentsForThisBlog = commentCount,
                    message = blog != null ? "Blog found" : "Blog not found"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/comments/debug
        [HttpGet("debug")]
        public async Task<IActionResult> DebugComments()
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var commentCount = await context.Comments.CountAsync();
                var blogCount = await context.Blogs.CountAsync();
                
                return Ok(new {
                    commentCount,
                    blogCount,
                    timestamp = DateTime.UtcNow,
                    message = "Comment debug info retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Debug failed", error = ex.Message });
            }
        }

        // ===== ROUTES CHUNG - PHẢI ĐẶT SAU =====

        // GET: api/comments/blog/{blogId}
        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult> GetCommentsByBlog(int blogId)
        {
            try
            {
                Console.WriteLine($"GetCommentsByBlog called for blogId: {blogId}");
                
                var comments = await _commentRepository.GetCommentsByBlogIdAsync(blogId);
                
                var commentDtos = comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.BlogId,
                    c.UserId,
                    c.GuestName,
                    c.GuestEmail,
                    User = c.User != null ? new 
                    { 
                        c.User.Id, 
                        Name = c.User.FullName, 
                        c.User.Email 
                    } : null,
                    AuthorName = c.User?.FullName ?? c.GuestName ?? "Anonymous",
                    IsGuest = c.UserId == null
                });

                Console.WriteLine($"Found {commentDtos.Count()} comments for blog {blogId}");
                return Ok(commentDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCommentsByBlog Error: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving comments", error = ex.Message });
            }
        }

        // POST: api/comments
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] CreateCommentRequest request)
        {
            try
            {
                Console.WriteLine($"CreateComment called - BlogId: {request.BlogId}, UserId: {request.UserId}, GuestName: {request.GuestName}");
                
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var blogExists = await context.Blogs.AnyAsync(b => b.Id == request.BlogId);
                if (!blogExists)
                {
                    return BadRequest(new { message = $"Blog with ID {request.BlogId} does not exist" });
                }

                if (request.UserId.HasValue && request.UserId.Value > 0)
                {
                    var userExists = await context.Users.AnyAsync(u => u.Id == request.UserId.Value);
                    if (!userExists)
                    {
                        return BadRequest(new { message = $"User with ID {request.UserId} does not exist" });
                    }
                }

                var isGuestComment = !request.UserId.HasValue || request.UserId.Value <= 0;
                
                var comment = new Comment
                {
                    Content = request.Content,
                    BlogId = request.BlogId,
                    UserId = isGuestComment ? null : request.UserId,
                    GuestName = isGuestComment ? (request.GuestName ?? "Anonymous") : null,
                    GuestEmail = isGuestComment ? request.GuestEmail : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdComment = await _commentRepository.CreateCommentAsync(comment);
                
                return Ok(new {
                    id = createdComment.Id,
                    content = createdComment.Content,
                    blogId = createdComment.BlogId,
                    userId = createdComment.UserId,
                    guestName = createdComment.GuestName,
                    guestEmail = createdComment.GuestEmail,
                    createdAt = createdComment.CreatedAt,
                    updatedAt = createdComment.UpdatedAt,
                    authorName = createdComment.User?.FullName ?? createdComment.GuestName ?? "Anonymous",
                    message = "Comment created successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateComment Error: {ex.Message}");
                return StatusCode(500, new { message = "Error creating comment", error = ex.Message });
            }
        }

        // DELETE: api/comments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id, [FromQuery] int? userId = null)
        {
            try
            {
                Console.WriteLine($"DeleteComment called for commentId: {id}, requestUserId: {userId}");
                
                var existingComment = await _commentRepository.GetCommentByIdAsync(id);
                if (existingComment == null)
                {
                    return NotFound(new { message = $"Comment with ID {id} not found" });
                }

                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var blog = await context.Blogs.FindAsync(existingComment.BlogId);
                if (blog == null)
                {
                    return BadRequest(new { message = "Blog not found" });
                }

                bool canDelete = false;
                string deleteReason = "";

                if (userId.HasValue && userId.Value > 0)
                {
                    if (blog.AuthorId == userId.Value)
                    {
                        canDelete = true;
                        deleteReason = "Blog author can delete any comment";
                    }
                    else if (existingComment.UserId == userId.Value)
                    {
                        canDelete = true;
                        deleteReason = "User can delete their own comment";
                    }
                }

                if (!canDelete)
                {
                    Console.WriteLine($"Delete denied: {deleteReason}");
                    return StatusCode(403, new { message = "You don't have permission to delete this comment", reason = deleteReason });
                }

                var success = await _commentRepository.DeleteCommentAsync(id);
                if (!success)
                {
                    return StatusCode(500, new { message = "Failed to delete comment" });
                }

                Console.WriteLine($"Comment deleted successfully with ID: {id}");
                
                return Ok(new { 
                    message = "Comment deleted successfully",
                    deletedCommentId = id,
                    reason = deleteReason
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteComment Error: {ex.Message}");
                return StatusCode(500, new { message = "Error deleting comment", error = ex.Message });
            }
        }

         // GET: api/comments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentByIdAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                return Ok(comment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetComment Error: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving comment", error = ex.Message });
            }
        }
    }

    // DTO Classes
    public class CreateCommentRequest
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public int BlogId { get; set; }
        
        public int? UserId { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
    }
}
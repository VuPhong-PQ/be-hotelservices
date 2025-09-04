using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(int blogId)
        {
            try
            {
                return await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Blog)
                    .Where(c => c.BlogId == blogId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCommentsByBlogIdAsync: {ex.Message}");
                return new List<Comment>();
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            try
            {
                return await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Blog)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCommentByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            try
            {
                comment.CreatedAt = DateTime.UtcNow;
                comment.UpdatedAt = DateTime.UtcNow;
                
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                
                // Return comment with includes
                return await GetCommentByIdAsync(comment.Id) ?? comment;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateCommentAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            try
            {
                comment.UpdatedAt = DateTime.UtcNow;
                
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
                
                return await GetCommentByIdAsync(comment.Id) ?? comment;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateCommentAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                    return false;

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteCommentAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Comments
                    .Include(c => c.User)
                    .Include(c => c.Blog)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCommentsByUserIdAsync: {ex.Message}");
                return new List<Comment>();
            }
        }

        public async Task<int> GetCommentCountByBlogIdAsync(int blogId)
        {
            try
            {
                return await _context.Comments
                    .Where(c => c.BlogId == blogId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCommentCountByBlogIdAsync: {ex.Message}");
                return 0;
            }
        }
    }
}

using HotelServiceAPI.Data;
using HotelServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelServiceAPI.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly HotelDbContext _context;

        public BlogRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetAllBlogsAsync()
        {
            try
            {
                return await _context.Blogs
                    .Include(b => b.Author)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBlogsAsync: {ex.Message}");
                return new List<Blog>();
            }
        }

        public async Task<Blog?> GetBlogByIdAsync(int id)
        {
            try
            {
                return await _context.Blogs
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBlogByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Blog>> GetBlogsByAuthorIdAsync(int authorId)
        {
            try
            {
                return await _context.Blogs
                    .Include(b => b.Author)
                    .Where(b => b.AuthorId == authorId)
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBlogsByAuthorIdAsync: {ex.Message}");
                return new List<Blog>();
            }
        }

        public async Task<Blog> CreateBlogAsync(Blog blog)
        {
            try
            {
                blog.CreatedAt = DateTime.UtcNow;
                blog.UpdatedAt = DateTime.UtcNow;
                
                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                
                // Return blog with Author included
                return await GetBlogByIdAsync(blog.Id) ?? blog;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateBlogAsync: {ex.Message}");
                throw; // Re-throw để controller catch được
            }
        }

        public async Task<Blog> UpdateBlogAsync(Blog blog)
        {
            try
            {
                blog.UpdatedAt = DateTime.UtcNow;
                
                _context.Blogs.Update(blog);
                await _context.SaveChangesAsync();
                
                return await GetBlogByIdAsync(blog.Id) ?? blog;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBlogAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBlogAsync(int id)
        {
            try
            {
                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                    return false;

                _context.Blogs.Remove(blog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteBlogAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Blog>> SearchBlogsAsync(string searchTerm)
        {
            try
            {
                return await _context.Blogs
                    .Include(b => b.Author)
                    .Where(b => b.Title.Contains(searchTerm) || b.Content.Contains(searchTerm))
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchBlogsAsync: {ex.Message}");
                return new List<Blog>();
            }
        }

        public async Task<IEnumerable<Blog>> GetRecentBlogsAsync(int count = 5)
        {
            try
            {
                return await _context.Blogs
                    .Include(b => b.Author)
                    .OrderByDescending(b => b.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRecentBlogsAsync: {ex.Message}");
                return new List<Blog>();
            }
        }
    }
}

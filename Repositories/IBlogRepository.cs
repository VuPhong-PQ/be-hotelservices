using HotelServiceAPI.Models;

namespace HotelServiceAPI.Repositories
{
    public interface IBlogRepository
    {
        Task<IEnumerable<Blog>> GetAllBlogsAsync();
        Task<Blog?> GetBlogByIdAsync(int id);
        Task<IEnumerable<Blog>> GetBlogsByAuthorIdAsync(int authorId);
        Task<Blog> CreateBlogAsync(Blog blog);
        Task<Blog> UpdateBlogAsync(Blog blog);
        Task<bool> DeleteBlogAsync(int id);
        Task<IEnumerable<Blog>> SearchBlogsAsync(string searchTerm);
        Task<IEnumerable<Blog>> GetRecentBlogsAsync(int count = 5);
    }
}

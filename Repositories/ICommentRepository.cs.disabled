using HotelServiceAPI.Models;

namespace HotelServiceAPI.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByBlogIdAsync(int blogId);
        Task<Comment?> GetCommentByIdAsync(int id);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int id);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
        Task<int> GetCommentCountByBlogIdAsync(int blogId);
    }
}

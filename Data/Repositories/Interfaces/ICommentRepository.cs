using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Data.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByBlogPostIdAsync(int blogPostId);
        Task<Comment> GetCommentByIdAsync(int id);
        Task CreateCommentAsync(Comment comment);
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int id);
        Task UpvoteCommentAsync(int id);
        Task ToggleUpvoteAsync(int commentId, string userId);
        Task FavoriteCommentAsync(int id);
    }
}
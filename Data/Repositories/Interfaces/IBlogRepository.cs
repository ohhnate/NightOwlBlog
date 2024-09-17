using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Data.Repositories.Interfaces
{
    public interface IBlogRepository
    {
        Task<IEnumerable<BlogPost>> GetAllPostsAsync();
        Task<BlogPost> GetPostByIdAsync(int id);
        Task<IEnumerable<BlogPost>> GetPostsByUserAsync(string username);
        Task<int> GetTotalCommentsByUserAsync(string username);
        Task<int> GetTotalViewsByUserAsync(string username);
        Task CreatePostAsync(BlogPost blogPost);
        Task UpdatePostAsync(BlogPost blogPost);
        Task DeletePostAsync(int id);
        Task IncrementViewCountAsync(int postId);
    }
}
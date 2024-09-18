using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Services.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogPost>> GetAllPostsAsync();
        Task<BlogPost> GetPostByIdAsync(int id);
        Task<IEnumerable<BlogPost>> GetPostsByUserAsync(string username);
        Task CreatePostAsync(BlogPost blogPost);
        Task UpdatePostAsync(BlogPost blogPost);
        Task DeletePostAsync(int id);
        Task IncrementViewCountAsync(int postId);
        Task<int> GetTotalCommentsByUserAsync(string username);
        Task<int> GetTotalViewsByUserAsync(string username);
        void InvalidatePostCache(int postId);
    }
}
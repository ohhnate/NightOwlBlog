using Microsoft.EntityFrameworkCore;
using SimpleBlogMVC.Data.Repositories.Interfaces;
using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Data.Repositories.Implementations
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogPost>> GetAllPostsAsync()
        {
            return await _context.BlogPosts.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            return await _context.BlogPosts.FindAsync(id);
        }

        public async Task<IEnumerable<BlogPost>> GetPostsByUserAsync(string username)
        {
            return await _context.BlogPosts
                .Where(p => p.Username == username)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalCommentsByUserAsync(string username)
        {
            return await _context.Comments
                .Where(c => c.BlogPost.Username == username)
                .CountAsync();
        }

        public async Task<int> GetTotalViewsByUserAsync(string username)
        {
            return await _context.BlogPosts
                .Where(p => p.Username == username)
                .SumAsync(p => p.Views);
        }

        public async Task CreatePostAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(BlogPost blogPost)
        {
            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            var post = await _context.BlogPosts.FindAsync(postId);
            if (post != null)
            {
                post.Views++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BlogSettings> GetBlogSettingsAsync(string userId)
        {
            return await _context.BlogSettings
                .FirstOrDefaultAsync(bs => bs.UserId == userId);
        }

        public async Task UpdateBlogSettingsAsync(BlogSettings settings)
        {
            _context.Entry(settings).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SimpleBlogMVC.Data;
using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Services
{
    public class BlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            return await _context.BlogPosts.ToListAsync();
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            return await _context.BlogPosts.FindAsync(id);
        }

        public async Task CreatePostAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(BlogPost blogPost)
        {
            var existingPost = await _context.BlogPosts.FindAsync(blogPost.Id);
            if (existingPost != null)
            {
                _context.Entry(existingPost).CurrentValues.SetValues(blogPost);
                await _context.SaveChangesAsync();
            }
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
    }
}
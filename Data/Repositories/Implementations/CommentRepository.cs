using Microsoft.EntityFrameworkCore;
using SimpleBlogMVC.Data.Repositories.Interfaces;
using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Data.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByBlogPostIdAsync(int blogPostId)
        {
            return await _context.Comments
                .Where(c => c.BlogPostId == blogPostId && !c.IsDeleted && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Replies.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.User)
                .OrderByDescending(c => c.IsFavorite)
                .ThenByDescending(c => c.UpvoteCount)
                .ThenByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpvoteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.UpvoteCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleUpvoteAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                throw new ArgumentException("Comment not found", nameof(commentId));
            }

            var upvote = await _context.CommentUpvotes
                .FirstOrDefaultAsync(u => u.CommentId == commentId && u.UserId == userId);

            if (upvote == null)
            {
                // Add new upvote
                _context.CommentUpvotes.Add(new CommentUpvote
                {
                    CommentId = commentId,
                    UserId = userId
                });
                comment.UpvoteCount++;
            }
            else
            {
                // Remove existing upvote
                _context.CommentUpvotes.Remove(upvote);
                comment.UpvoteCount--;
            }

            await _context.SaveChangesAsync();
        }

        public async Task FavoriteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsFavorite = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
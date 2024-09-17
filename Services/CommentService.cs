using SimpleBlogMVC.Data.Repositories.Interfaces;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services.Interfaces;

namespace SimpleBlogMVC.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByBlogPostIdAsync(int blogPostId)
        {
            return await _commentRepository.GetCommentsByBlogPostIdAsync(blogPostId);
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _commentRepository.GetCommentByIdAsync(id);
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            try
            {
                await _commentRepository.CreateCommentAsync(comment);
                _logger.LogInformation($"Comment created successfully for post {comment.BlogPostId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating comment for post {comment.BlogPostId}");
                throw;
            }
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.UpdateCommentAsync(comment);
        }

        public async Task DeleteCommentAsync(int id)
        {
            await _commentRepository.DeleteCommentAsync(id);
        }

        public async Task UpvoteCommentAsync(int id)
        {
            await _commentRepository.UpvoteCommentAsync(id);
        }

        public async Task ToggleUpvoteAsync(int commentId, string userId)
        {
            try
            {
                await _commentRepository.ToggleUpvoteAsync(commentId, userId);
                _logger.LogInformation($"Upvote toggled for comment {commentId} by user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling upvote for comment {commentId} by user {userId}");
                throw;
            }
        }

        public async Task FavoriteCommentAsync(int id)
        {
            await _commentRepository.FavoriteCommentAsync(id);
        }
    }
}
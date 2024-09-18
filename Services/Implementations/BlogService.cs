using SimpleBlogMVC.Data;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services.Interfaces;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace SimpleBlogMVC.Services.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BlogService> _logger;
        private readonly ICacheService _cacheService;
        private readonly HtmlSanitizer _htmlSanitizer;

        public BlogService(
            IUnitOfWork unitOfWork,
            ILogger<BlogService> logger,
            ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cacheService = cacheService;
            _htmlSanitizer = new HtmlSanitizer();
            ConfigureHtmlSanitizer();
        }

        private void ConfigureHtmlSanitizer()
        {
            _htmlSanitizer.AllowedTags.Add("iframe");
            _htmlSanitizer.AllowedAttributes.Add("allow");
            _htmlSanitizer.AllowedAttributes.Add("allowfullscreen");
            _htmlSanitizer.AllowedCssProperties.Add("width");
            _htmlSanitizer.AllowedCssProperties.Add("height");
            _htmlSanitizer.AllowedSchemes.Add("data");
        }

        public void InvalidatePostCache(int postId)
        {
            string cacheKey = $"Post_{postId}";
            _cacheService.Remove(cacheKey);
        }

        public async Task<IEnumerable<BlogPost>> GetAllPostsAsync()
        {
            const string cacheKey = "AllPosts";
            if (!_cacheService.TryGet(cacheKey, out IEnumerable<BlogPost> posts))
            {
                try
                {
                    posts = await _unitOfWork.BlogRepository.GetAllPostsAsync();
                    _cacheService.Set(cacheKey, posts, TimeSpan.FromMinutes(10));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching all posts");
                    throw;
                }
            }
            return posts;
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            string cacheKey = $"Post_{id}";
            if (!_cacheService.TryGet(cacheKey, out BlogPost post))
            {
                try
                {
                    post = await _unitOfWork.BlogRepository.GetPostByIdAsync(id);
                    if (post != null)
                    {
                        await _unitOfWork.Context.Entry(post)
                            .Collection(b => b.Comments)
                            .Query()
                            .Include(c => c.User)
                            .LoadAsync();
                        _cacheService.Set(cacheKey, post, TimeSpan.FromMinutes(10));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred while fetching post with id {id}");
                    throw;
                }
            }
            return post;
        }

        public async Task<IEnumerable<BlogPost>> GetPostsByUserAsync(string username)
        {
            try
            {
                return await _unitOfWork.BlogRepository.GetPostsByUserAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching posts for user {username}");
                throw;
            }
        }

        public async Task CreatePostAsync(BlogPost blogPost)
        {
            try
            {
                blogPost.Content = _htmlSanitizer.Sanitize(blogPost.Content);
                await _unitOfWork.BlogRepository.CreatePostAsync(blogPost);
                await _unitOfWork.CompleteAsync();
                _cacheService.Remove("AllPosts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new post");
                throw;
            }
        }

        public async Task UpdatePostAsync(BlogPost blogPost)
        {
            try
            {
                blogPost.Content = _htmlSanitizer.Sanitize(blogPost.Content);
                await _unitOfWork.BlogRepository.UpdatePostAsync(blogPost);
                await _unitOfWork.CompleteAsync();
                _cacheService.Remove("AllPosts");
                _cacheService.Remove($"Post_{blogPost.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating post with id {blogPost.Id}");
                throw;
            }
        }

        public async Task DeletePostAsync(int id)
        {
            try
            {
                await _unitOfWork.BlogRepository.DeletePostAsync(id);
                await _unitOfWork.CompleteAsync();
                _cacheService.Remove("AllPosts");
                _cacheService.Remove($"Post_{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting post with id {id}");
                throw;
            }
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            try
            {
                await _unitOfWork.BlogRepository.IncrementViewCountAsync(postId);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while incrementing view count for post with id {postId}");
                throw;
            }
        }

        public async Task<int> GetTotalCommentsByUserAsync(string username)
        {
            try
            {
                return await _unitOfWork.BlogRepository.GetTotalCommentsByUserAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching total comments for user {username}");
                throw;
            }
        }

        public async Task<int> GetTotalViewsByUserAsync(string username)
        {
            try
            {
                return await _unitOfWork.BlogRepository.GetTotalViewsByUserAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching total views for user {username}");
                throw;
            }
        }
    }
}
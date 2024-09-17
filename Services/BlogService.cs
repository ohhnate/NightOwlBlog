using SimpleBlogMVC.Data;
using SimpleBlogMVC.Models;
using Ganss.Xss;
using SimpleBlogMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SimpleBlogMVC.Services
{
    public class BlogService : BaseService
    {
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly ICacheService _cacheService;

        public BlogService(IUnitOfWork unitOfWork, ILogger<BlogService> logger, ICacheService cacheService)
            : base(logger, unitOfWork)
        {
            _cacheService = cacheService;
            _htmlSanitizer = new HtmlSanitizer();
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
            string cacheKey = "AllPosts";
            if (!_cacheService.TryGet(cacheKey, out IEnumerable<BlogPost> posts))
            {
                posts = await ExecuteAsync(() => _unitOfWork.BlogRepository.GetAllPostsAsync(),
                    "Error occurred while fetching all posts");
                _cacheService.Set(cacheKey, posts, TimeSpan.FromMinutes(10));
            }
            return posts;
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            string cacheKey = $"Post_{id}";
            if (!_cacheService.TryGet(cacheKey, out BlogPost post))
            {
                post = await ExecuteAsync(async () =>
                {
                    var dbPost = await _unitOfWork.BlogRepository.GetPostByIdAsync(id);
                    if (dbPost != null)
                    {
                        // Ensure comments are loaded
                        await _unitOfWork.Context.Entry(dbPost)
                            .Collection(b => b.Comments)
                            .Query()
                            .Include(c => c.User)
                            .LoadAsync();
                    }
                    return dbPost;
                }, $"Error occurred while fetching post with id {id}");

                if (post != null)
                {
                    _cacheService.Set(cacheKey, post, TimeSpan.FromMinutes(10));
                }
            }
            return post;
        }

        public async Task<IEnumerable<BlogPost>> GetPostsByUserAsync(string username)
        {
            return await ExecuteAsync(() => _unitOfWork.BlogRepository.GetPostsByUserAsync(username),
                $"Error occurred while fetching posts for user {username}");
        }

        public async Task CreatePostAsync(BlogPost blogPost)
        {
            blogPost.Content = _htmlSanitizer.Sanitize(blogPost.Content);
            await ExecuteAsync(() => _unitOfWork.BlogRepository.CreatePostAsync(blogPost),
                "Error occurred while creating a new post");

            await _unitOfWork.CompleteAsync();
            _cacheService.Remove("AllPosts");
        }

        public async Task UpdatePostAsync(BlogPost blogPost)
        {
            blogPost.Content = _htmlSanitizer.Sanitize(blogPost.Content);
            await ExecuteAsync(() => _unitOfWork.BlogRepository.UpdatePostAsync(blogPost),
                $"Error occurred while updating post with id {blogPost.Id}");

            await _unitOfWork.CompleteAsync();
            _cacheService.Remove("AllPosts");
            _cacheService.Remove($"Post_{blogPost.Id}");
        }

        public async Task DeletePostAsync(int id)
        {
            await ExecuteAsync(() => _unitOfWork.BlogRepository.DeletePostAsync(id),
                $"Error occurred while deleting post with id {id}");

            await _unitOfWork.CompleteAsync();
            _cacheService.Remove("AllPosts");
            _cacheService.Remove($"Post_{id}");
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            await ExecuteAsync(() => _unitOfWork.BlogRepository.IncrementViewCountAsync(postId),
                $"Error occurred while incrementing view count for post with id {postId}");

            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> GetTotalCommentsByUserAsync(string username)
        {
            return await ExecuteAsync(() => _unitOfWork.BlogRepository.GetTotalCommentsByUserAsync(username),
                $"Error occurred while fetching total comments for user {username}");
        }

        public async Task<int> GetTotalViewsByUserAsync(string username)
        {
            return await ExecuteAsync(() => _unitOfWork.BlogRepository.GetTotalViewsByUserAsync(username),
                $"Error occurred while fetching total views for user {username}");
        }
    }
}
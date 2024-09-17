using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services;
using Microsoft.AspNetCore.Identity;
using SimpleBlogMVC.Services.Interfaces;

namespace SimpleBlogMVC.Controllers
{
    public class BlogController : BaseController
    {
        private readonly BlogService _blogService;
        private readonly ICommentService _commentService;

        public BlogController(
            BlogService blogService,
            ICommentService commentService,
            ILogger<BlogController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
            : base(logger, userManager, signInManager)
        {
            _blogService = blogService;
            _commentService = commentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var posts = await _blogService.GetAllPostsAsync();
                return View(posts);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Error occurred while fetching blog posts.");
            }
        }

        [Authorize]
        public IActionResult Create()
        {
            return View("Upsert", new BlogPost());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost blogPost)
        {
            if (!ModelState.IsValid)
            {
                return View("Upsert", blogPost);
            }

            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found. Please try logging in again.");
                    return View("Upsert", blogPost);
                }

                blogPost.Username = user.UserName;
                blogPost.Comments = new List<Comment>();
                await _blogService.CreatePostAsync(blogPost);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, "Error occurred while creating blog post.");
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var post = await _blogService.GetPostByIdAsync(id);
                if (post == null || !await IsUserOwnerAsync(post.Username))
                {
                    return Forbid();
                }

                return View("Upsert", post);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"Error occurred while fetching blog post with id {id} for editing.");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View("Upsert", blogPost);
            }

            try
            {
                var existingPost = await _blogService.GetPostByIdAsync(id);
                if (!await IsUserOwnerAsync(existingPost.Username))
                {
                    return Forbid();
                }

                await _blogService.UpdatePostAsync(blogPost);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"Error occurred while editing blog post with id {id}.");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var post = await _blogService.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }

                if (post.Comments == null)
                {
                    post.Comments = new List<Comment>();
                }

                await _blogService.IncrementViewCountAsync(id);

                return View(post);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"Error occurred while fetching blog post with id {id}.");
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var post = await _blogService.GetPostByIdAsync(id);
                if (post == null || !await IsUserOwnerAsync(post.Username))
                {
                    return Forbid();
                }

                return View(post);
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"Error occurred while fetching blog post with id {id} for deletion.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var post = await _blogService.GetPostByIdAsync(id);
                if (post == null || !await IsUserOwnerAsync(post.Username))
                {
                    return Forbid();
                }

                await _blogService.DeletePostAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"Error occurred while deleting blog post with id {id}.");
            }
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int blogPostId, string content, int? parentCommentId)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    _logger.LogWarning("AddComment: User not found");
                    return Unauthorized();
                }

                var comment = new Comment
                {
                    BlogPostId = blogPostId,
                    Content = content,
                    UserId = user.Id,
                    ParentCommentId = parentCommentId
                };

                await _commentService.CreateCommentAsync(comment);
                _logger.LogInformation($"Comment added successfully for post {blogPostId}");

                // Invalidate cache for this blog post
                _blogService.InvalidatePostCache(blogPostId);

                return RedirectToAction(nameof(Details), new { id = blogPostId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding comment to post {blogPostId}");
                return HandleException(ex, "An error occurred while adding the comment.");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, string content)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            if (user == null || user.Id != comment.UserId)
            {
                return Unauthorized();
            }

            comment.Content = content;
            await _commentService.UpdateCommentAsync(comment);
            return RedirectToAction(nameof(Details), new { id = comment.BlogPostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var post = await _blogService.GetPostByIdAsync(comment.BlogPostId);
            if (user == null || (user.Id != comment.UserId && user.UserName != post.Username))
            {
                return Unauthorized();
            }

            await _commentService.DeleteCommentAsync(id);
            return RedirectToAction(nameof(Details), new { id = comment.BlogPostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpvoteComment(int id)
        {
            await _commentService.UpvoteCommentAsync(id);
            var comment = await _commentService.GetCommentByIdAsync(id);
            return RedirectToAction(nameof(Details), new { id = comment.BlogPostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUpvote(int id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            await _commentService.ToggleUpvoteAsync(id, user.Id);

            var comment = await _commentService.GetCommentByIdAsync(id);
            return RedirectToAction(nameof(Details), new { id = comment.BlogPostId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavoriteComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            var post = await _blogService.GetPostByIdAsync(comment.BlogPostId);
            if (user == null || user.UserName != post.Username)
            {
                return Unauthorized();
            }

            await _commentService.FavoriteCommentAsync(id);
            return RedirectToAction(nameof(Details), new { id = comment.BlogPostId });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services.Interfaces;

namespace SimpleBlogMVC.Controllers
{
    public class CommentController : BaseController
    {
        private readonly ICommentService _commentService;
        private readonly IBlogService _blogService;

        public CommentController(
            ICommentService commentService,
            IBlogService blogService,
            ILogger<CommentController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
            : base(logger, userManager, signInManager)
        {
            _commentService = commentService;
            _blogService = blogService;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int blogPostId, string content, int? parentCommentId)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
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
                _blogService.InvalidatePostCache(blogPostId);

                return RedirectToAction("Details", "Blog", new { id = blogPostId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the comment.");
                return RedirectToAction("Details", "Blog", new { id = blogPostId });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string content)
        {
            try
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
                _blogService.InvalidatePostCache(comment.BlogPostId);

                return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while editing the comment.");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
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
                _blogService.InvalidatePostCache(comment.BlogPostId);

                return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while deleting the comment.");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUpvote(int id)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                await _commentService.ToggleUpvoteAsync(id, user.Id);

                var comment = await _commentService.GetCommentByIdAsync(id);
                _blogService.InvalidatePostCache(comment.BlogPostId);

                return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while toggling the upvote.");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FavoriteComment(int id)
        {
            try
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
                _blogService.InvalidatePostCache(comment.BlogPostId);

                return RedirectToAction("Details", "Blog", new { id = comment.BlogPostId });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while favoriting the comment.");
            }
        }
    }
}
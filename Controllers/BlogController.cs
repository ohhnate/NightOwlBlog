using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services.Interfaces;

namespace SimpleBlogMVC.Controllers
{
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(
            IBlogService blogService,
            ILogger<BlogController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
            : base(logger, userManager, signInManager)
        {
            _blogService = blogService;
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
                if (existingPost == null || !await IsUserOwnerAsync(existingPost.Username))
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
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services;

namespace SimpleBlogMVC.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly BlogService _blogService;

        public DashboardController(
            ILogger<DashboardController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            BlogService blogService)
            : base(logger, userManager, signInManager)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var userPosts = await _blogService.GetPostsByUserAsync(user.UserName);
                var viewModel = new DashboardViewModel
                {
                    User = user,
                    RecentPosts = userPosts,
                    TotalPosts = userPosts.Count(),
                    TotalComments = await _blogService.GetTotalCommentsByUserAsync(user.UserName),
                    TotalViews = await _blogService.GetTotalViewsByUserAsync(user.UserName)
                };
                viewModel.UpdateFromUser(user);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading dashboard.");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(DashboardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var user = await GetCurrentUserAsync();
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                model.UpdateUser(user);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating your profile. Please try again.");
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
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
                _logger.LogError(ex, $"Error occurred while deleting blog post with id {id}.");
                return View("Error");
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);
    }
}
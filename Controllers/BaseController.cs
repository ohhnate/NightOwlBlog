using Microsoft.AspNetCore.Mvc;
using SimpleBlogMVC.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace SimpleBlogMVC.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;

        protected BaseController(
            ILogger logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected IActionResult HandleException(Exception ex, string errorMessage = "An unexpected error occurred.")
        {
            _logger.LogError(ex, errorMessage);
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = errorMessage
            };
            return View("Error", errorViewModel);
        }

        protected bool IsUserInRole(string role)
        {
            return User.Identity.IsAuthenticated && User.IsInRole(role);
        }

        protected async Task<bool> IsUserOwnerAsync(string resourceOwnerUsername)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return currentUser != null && currentUser.UserName == resourceOwnerUsername;
        }

        protected IActionResult RedirectWithMessage(string action, string controller, string message, LogLevel logLevel = LogLevel.Information)
        {
            _logger.Log(logLevel, message);
            TempData["StatusMessage"] = message;
            return RedirectToAction(action, controller);
        }

        protected async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }

        protected async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);
    }
}
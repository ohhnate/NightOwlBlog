using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBlogMVC.Models;
using System.Threading.Tasks;

namespace SimpleBlogMVC.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
            : base(logger, userManager, signInManager)
        {
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email address is already in use.");
                    return View(model);
                }

                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, DisplayName = model.Username };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User created a new account with password. Username: {user.UserName}");

                    // Automatically confirm the email
                    await _userManager.ConfirmEmailAsync(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning($"User registration error: {error.Description}");
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Attempting to log in user: {model.UsernameOrEmail}");
                var user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
                }

                if (user != null)
                {
                    _logger.LogInformation($"User found: {user.UserName}");

                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {user.UserName} logged in successfully.");
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning($"User account {user.UserName} locked out.");
                        return RedirectToAction("Lockout");
                    }
                    if (result.IsNotAllowed)
                    {
                        _logger.LogWarning($"Login not allowed for user: {user.UserName}. Email confirmed: {user.EmailConfirmed}");
                        ModelState.AddModelError(string.Empty, "Unable to log in. Please contact support.");
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid password for user: {user.UserName}");
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }
                }
                else
                {
                    _logger.LogWarning($"User not found: {model.UsernameOrEmail}");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }
    }
}
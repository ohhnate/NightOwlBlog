using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Data;
using System.Text;

namespace SimpleBlogMVC.Controllers
{
    public class TestDataController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TestDataController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTestData(int numberOfUsers = 10, int postsPerUser = 5)
        {
            var result = new StringBuilder();

            // Generate test users
            for (int i = 1; i <= numberOfUsers; i++)
            {
                var user = new ApplicationUser
                {
                    UserName = $"testuser{i}",
                    Email = $"testuser{i}@example.com",
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, "TestPassword123!");

                if (createUserResult.Succeeded)
                {
                    result.AppendLine($"Created user: {user.UserName}");

                    // Generate blog posts for each user
                    for (int j = 1; j <= postsPerUser; j++)
                    {
                        var post = new BlogPost
                        {
                            Title = $"Test Post {j} by {user.UserName}",
                            Content = $"This is test content for post {j} by {user.UserName}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                            Username = user.UserName,
                            CreatedAt = DateTime.Now.AddDays(-j),
                            Tags = $"test,user{i},post{j}"
                        };

                        _context.BlogPosts.Add(post);
                        result.AppendLine($"Created post: {post.Title}");
                    }
                }
                else
                {
                    result.AppendLine($"Failed to create user: {user.UserName}");
                }
            }

            await _context.SaveChangesAsync();

            ViewBag.Result = result.ToString();
            return View("Index");
        }
    }
}
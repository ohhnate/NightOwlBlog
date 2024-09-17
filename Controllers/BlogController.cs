using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SimpleBlogMVC.Models;
using SimpleBlogMVC.Services;
using Ganss.Xss;

namespace SimpleBlogMVC.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;
        private readonly ILogger<BlogController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;

        public BlogController(BlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
            _htmlSanitizer = new HtmlSanitizer();
            _htmlSanitizer.AllowedTags.Add("iframe");
            _htmlSanitizer.AllowedAttributes.Add("allow");
            _htmlSanitizer.AllowedAttributes.Add("allowfullscreen");
            _htmlSanitizer.AllowedCssProperties.Add("width");
            _htmlSanitizer.AllowedCssProperties.Add("height");
            _htmlSanitizer.AllowedSchemes.Add("data");
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _blogService.GetAllPostsAsync();
            return View(posts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
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
            ModelState.Remove("Username");

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.Username = User.Identity.Name;
                    // Sanitize the HTML content
                    blogPost.Content = SanitizeHtml(blogPost.Content);
                    await _blogService.CreatePostAsync(blogPost);
                    _logger.LogInformation("Blog post created successfully");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while creating blog post");
                    ModelState.AddModelError("", "An error occurred while creating the blog post. Please try again.");
                }
            }
            else
            {
                _logger.LogWarning("Invalid ModelState when creating blog post");
            }
            return View("Upsert", blogPost);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.Username != User.Identity.Name)
            {
                return Forbid();
            }
            return View("Upsert", post);
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

            if (ModelState.IsValid)
            {
                var existingPost = await _blogService.GetPostByIdAsync(id);
                if (existingPost.Username != User.Identity.Name)
                {
                    return Forbid();
                }
                blogPost.Username = User.Identity.Name;
                // Sanitize the HTML content
                blogPost.Content = SanitizeHtml(blogPost.Content);
                await _blogService.UpdatePostAsync(blogPost);
                return RedirectToAction(nameof(Index));
            }
            return View("Upsert", blogPost);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.Username != User.Identity.Name)
            {
                return Forbid();
            }
            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (post.Username != User.Identity.Name)
            {
                return Forbid();
            }
            await _blogService.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private string SanitizeHtml(string html)
        {
            return _htmlSanitizer.Sanitize(html);
        }
    }
}
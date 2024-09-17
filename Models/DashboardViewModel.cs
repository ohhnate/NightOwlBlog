using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<BlogPost> RecentPosts { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public int TotalViews { get; set; }

        // Profile settings
        [Display(Name = "Display Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string DisplayName { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string Bio { get; set; }

        [Display(Name = "Profile Picture")]
        public string AvatarUrl { get; set; }

        [Display(Name = "Website URL")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string WebsiteUrl { get; set; }

        [Display(Name = "Twitter Handle")]
        [RegularExpression(@"^@?(\w){1,15}$", ErrorMessage = "Please enter a valid Twitter handle.")]
        public string TwitterHandle { get; set; }

        // Account settings
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }

        [Display(Name = "Enable Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        // Blog settings
        [Display(Name = "Allow Comments")]
        public bool AllowComments { get; set; }

        [Display(Name = "Moderate Comments")]
        public bool ModerateComments { get; set; }

        [Display(Name = "Posts Per Page")]
        [Range(1, 50, ErrorMessage = "Please enter a value between 1 and 50.")]
        public int PostsPerPage { get; set; }

        [Display(Name = "Default Post Category")]
        [StringLength(50, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string DefaultPostCategory { get; set; }

        public void UpdateFromUser(ApplicationUser user)
        {
            DisplayName = user.DisplayName;
            Bio = user.Bio;
            AvatarUrl = user.AvatarUrl;
            WebsiteUrl = user.WebsiteUrl;
            TwitterHandle = user.TwitterHandle;
            Email = user.Email;
            TwoFactorEnabled = user.TwoFactorEnabled;
        }

        public void UpdateUser(ApplicationUser user)
        {
            user.DisplayName = DisplayName;
            user.Bio = Bio;
            user.AvatarUrl = AvatarUrl;
            user.WebsiteUrl = WebsiteUrl;
            user.TwitterHandle = TwitterHandle;
            // Email and TwoFactorEnabled should be updated separately using UserManager
        }
    }
}
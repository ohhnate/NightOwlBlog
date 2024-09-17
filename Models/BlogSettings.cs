using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class BlogSettings
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool AllowComments { get; set; } = true;

        public bool ModerateComments { get; set; } = false;

        [Range(1, 50)]
        public int PostsPerPage { get; set; } = 10;

        [StringLength(50)]
        public string DefaultPostCategory { get; set; } = "Uncategorized";

        public ApplicationUser User { get; set; }
    }
}
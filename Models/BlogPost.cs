using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class BlogPost
    {
        public BlogPost()
        {
            Comments = new List<Comment>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public string? Username { get; set; }

        [Display(Name = "Tags")]
        [RegularExpression(@"^[\w\s,]+$", ErrorMessage = "Tags can only contain letters, numbers, spaces, and commas")]
        [StringLength(500, ErrorMessage = "Tags cannot be longer than 500 characters")]
        public string? Tags { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Views must be a non-negative number")]
        public int Views { get; set; } = 0;

        public ICollection<Comment> Comments { get; set; }
    }
}
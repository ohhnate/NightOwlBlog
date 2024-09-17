using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        public string? Username { get; set; }

        [Display(Name = "Tags")]
        public string? Tags { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
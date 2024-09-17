using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public string Username { get; set; }

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
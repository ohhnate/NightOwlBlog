using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }

        public ICollection<Comment> Replies { get; set; }

        public int UpvoteCount { get; set; } = 0;

        public bool IsFavorite { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public ICollection<CommentUpvote> Upvotes { get; set; }
    }
}
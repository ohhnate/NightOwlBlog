using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleBlogMVC.Models;

namespace SimpleBlogMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogSettings> BlogSettings { get; set; }
        public DbSet<CommentUpvote> CommentUpvotes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogPost>()
                .Property(b => b.Tags)
                .HasMaxLength(5000);

            builder.Entity<ApplicationUser>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommentUpvote>()
                .HasKey(cu => new { cu.CommentId, cu.UserId });

            builder.Entity<CommentUpvote>()
                .HasOne(cu => cu.Comment)
                .WithMany(c => c.Upvotes)
                .HasForeignKey(cu => cu.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommentUpvote>()
                .HasOne(cu => cu.User)
                .WithMany()
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
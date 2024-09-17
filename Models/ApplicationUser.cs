using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlogMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? DisplayName { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(4000)]
        public string? AvatarUrl { get; set; }

        [StringLength(100)]
        public string? WebsiteUrl { get; set; }

        [StringLength(50)]
        public string? TwitterHandle { get; set; }
    }
}
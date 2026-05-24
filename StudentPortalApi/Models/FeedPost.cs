using System;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalApi.Models
{
    public class FeedPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentPortalApi.Models
{
    public class VideoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }
        public string? FileUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [JsonIgnore]
        public User? CreatedBy { get; set; }
    }
}

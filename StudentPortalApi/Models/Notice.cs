using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentPortalApi.Models
{
    public class Notice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int CreatedById { get; set; }

        [JsonIgnore]
        public User? CreatedBy { get; set; }
    }
}

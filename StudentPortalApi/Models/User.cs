using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentPortalApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "student";

        public string? ProfilePictureUrl { get; set; }
    }
}

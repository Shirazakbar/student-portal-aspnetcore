using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudentPortalApi.Models
{
    public class PostReaction
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }

        public int FeedPostId { get; set; }
        [JsonIgnore]
        public FeedPost? FeedPost { get; set; }

        // true = Like, false = Dislike
        public bool IsLike { get; set; }
    }
}

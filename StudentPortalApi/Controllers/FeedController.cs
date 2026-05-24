using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;

namespace StudentPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public FeedController(StudentPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.FeedPosts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.CreatedAt,
                    User = new { p.User!.Id, p.User.Name, p.User.ProfilePictureUrl },
                    Likes = _context.PostReactions.Count(r => r.FeedPostId == p.Id && r.IsLike),
                    Dislikes = _context.PostReactions.Count(r => r.FeedPostId == p.Id && !r.IsLike)
                })
                .ToListAsync();

            return Ok(posts);
        }

        public class CreatePostDto
        {
            public int UserId { get; set; }
            public string Content { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto req)
        {
            var user = await _context.Users.FindAsync(req.UserId);
            if (user == null) return NotFound("User not found.");

            var post = new FeedPost
            {
                Content = req.Content,
                UserId = req.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.FeedPosts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                post.Id,
                post.Content,
                post.CreatedAt,
                User = new { user.Id, user.Name, user.ProfilePictureUrl },
                Likes = 0,
                Dislikes = 0
            });
        }

        public class ReactDto
        {
            public int UserId { get; set; }
            public bool IsLike { get; set; }
        }

        [HttpPost("{postId}/react")]
        public async Task<IActionResult> ReactToPost(int postId, [FromBody] ReactDto req)
        {
            var reaction = await _context.PostReactions
                .FirstOrDefaultAsync(r => r.FeedPostId == postId && r.UserId == req.UserId);

            if (reaction == null)
            {
                reaction = new PostReaction { FeedPostId = postId, UserId = req.UserId, IsLike = req.IsLike };
                _context.PostReactions.Add(reaction);
            }
            else
            {
                if (reaction.IsLike == req.IsLike)
                {
                    _context.PostReactions.Remove(reaction); // Toggle off
                }
                else
                {
                    reaction.IsLike = req.IsLike; // Switch from like to dislike or vice-versa
                }
            }

            await _context.SaveChangesAsync();

            var likes = await _context.PostReactions.CountAsync(r => r.FeedPostId == postId && r.IsLike);
            var dislikes = await _context.PostReactions.CountAsync(r => r.FeedPostId == postId && !r.IsLike);

            return Ok(new { likes, dislikes });
        }
    }
}

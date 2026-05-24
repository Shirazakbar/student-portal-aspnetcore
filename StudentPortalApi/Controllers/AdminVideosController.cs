using Microsoft.AspNetCore.Mvc;
using StudentPortalApi.Attributes;
using StudentPortalApi.Models;
using StudentPortalApi.Services;

namespace StudentPortalApi.Controllers
{
    [Route("api/admin/videos")]
    [ApiController]
    [AdminAuthorize]
    public class AdminVideosController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public AdminVideosController(StudentPortalContext context)
        {
            _context = context;
        }

        public class VideoRequest
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string? VideoUrl { get; set; }
            public IFormFile? File { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVideo([FromForm] VideoRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
            {
                return BadRequest("Title is required.");
            }

            var currentUser = HttpContext.Items["CurrentUser"] as User;
            if (currentUser == null) return Unauthorized();

            var video = new VideoItem
            {
                Title = req.Title,
                Description = req.Description,
                VideoUrl = req.VideoUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = currentUser.Id
            };

            if (req.File != null && req.File.Length > 0)
            {
                video.FileUrl = await FileUploadService.SaveFileAsync(req.File);
            }

            _context.VideoItems.Add(video);
            await _context.SaveChangesAsync();
            return Ok(video);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVideo(int id, [FromForm] VideoRequest req)
        {
            var video = await _context.VideoItems.FindAsync(id);
            if (video == null) return NotFound();

            video.Title = string.IsNullOrWhiteSpace(req.Title) ? video.Title : req.Title;
            video.Description = req.Description ?? video.Description;
            video.VideoUrl = string.IsNullOrWhiteSpace(req.VideoUrl) ? video.VideoUrl : req.VideoUrl;
            video.UpdatedAt = DateTime.UtcNow;

            if (req.File != null && req.File.Length > 0)
            {
                video.FileUrl = await FileUploadService.SaveFileAsync(req.File);
            }

            await _context.SaveChangesAsync();
            return Ok(video);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _context.VideoItems.FindAsync(id);
            if (video == null) return NotFound();

            _context.VideoItems.Remove(video);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

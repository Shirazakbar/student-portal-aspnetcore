using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;

namespace StudentPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public VideosController(StudentPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
            var videos = await _context.VideoItems
                .Include(v => v.CreatedBy)
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => new
                {
                    v.Id,
                    v.Title,
                    v.Description,
                    v.VideoUrl,
                    v.FileUrl,
                    v.CreatedAt,
                    v.UpdatedAt,
                    CreatedBy = v.CreatedBy == null ? null : new { v.CreatedBy.Id, v.CreatedBy.Name }
                })
                .ToListAsync();

            return Ok(videos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(int id)
        {
            var video = await _context.VideoItems
                .Include(v => v.CreatedBy)
                .Where(v => v.Id == id)
                .Select(v => new
                {
                    v.Id,
                    v.Title,
                    v.Description,
                    v.VideoUrl,
                    v.FileUrl,
                    v.CreatedAt,
                    v.UpdatedAt,
                    CreatedBy = v.CreatedBy == null ? null : new { v.CreatedBy.Id, v.CreatedBy.Name }
                })
                .FirstOrDefaultAsync();

            if (video == null) return NotFound();
            return Ok(video);
        }
    }
}

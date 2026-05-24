using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;

namespace StudentPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticesController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public NoticesController(StudentPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotices()
        {
            var notices = await _context.Notices
                .Include(n => n.CreatedBy)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.CreatedAt,
                    n.UpdatedAt,
                    CreatedBy = n.CreatedBy == null ? null : new { n.CreatedBy.Id, n.CreatedBy.Name }
                })
                .ToListAsync();

            return Ok(notices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotice(int id)
        {
            var notice = await _context.Notices
                .Include(n => n.CreatedBy)
                .Where(n => n.Id == id)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.CreatedAt,
                    n.UpdatedAt,
                    CreatedBy = n.CreatedBy == null ? null : new { n.CreatedBy.Id, n.CreatedBy.Name }
                })
                .FirstOrDefaultAsync();

            if (notice == null) return NotFound();
            return Ok(notice);
        }
    }
}

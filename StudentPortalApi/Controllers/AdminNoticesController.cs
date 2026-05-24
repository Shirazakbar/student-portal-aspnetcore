using Microsoft.AspNetCore.Mvc;
using StudentPortalApi.Attributes;
using StudentPortalApi.Models;

namespace StudentPortalApi.Controllers
{
    [Route("api/admin/notices")]
    [ApiController]
    [AdminAuthorize]
    public class AdminNoticesController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public AdminNoticesController(StudentPortalContext context)
        {
            _context = context;
        }

        public class NoticeRequest
        {
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotice([FromBody] NoticeRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Content))
            {
                return BadRequest("Title and content are required.");
            }

            var currentUser = HttpContext.Items["CurrentUser"] as User;
            if (currentUser == null) return Unauthorized();

            var notice = new Notice
            {
                Title = req.Title,
                Content = req.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = currentUser.Id
            };

            _context.Notices.Add(notice);
            await _context.SaveChangesAsync();

            return Ok(notice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotice(int id, [FromBody] NoticeRequest req)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice == null) return NotFound();

            notice.Title = string.IsNullOrWhiteSpace(req.Title) ? notice.Title : req.Title;
            notice.Content = string.IsNullOrWhiteSpace(req.Content) ? notice.Content : req.Content;
            notice.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(notice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotice(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice == null) return NotFound();

            _context.Notices.Remove(notice);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using StudentPortalApi.Attributes;
using StudentPortalApi.Models;
using StudentPortalApi.Services;

namespace StudentPortalApi.Controllers
{
    [Route("api/admin/assignments")]
    [ApiController]
    [AdminAuthorize]
    public class AdminAssignmentsController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public AdminAssignmentsController(StudentPortalContext context)
        {
            _context = context;
        }

        public class AssignmentRequest
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }
            public IFormFile? Attachment { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromForm] AssignmentRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Title))
            {
                return BadRequest("Title is required.");
            }

            var currentUser = HttpContext.Items["CurrentUser"] as User;
            if (currentUser == null) return Unauthorized();

            var assignment = new Assignment
            {
                Title = req.Title,
                Description = req.Description,
                DueDate = req.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = currentUser.Id
            };

            if (req.Attachment != null && req.Attachment.Length > 0)
            {
                assignment.AttachmentUrl = await FileUploadService.SaveFileAsync(req.Attachment);
            }

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok(assignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromForm] AssignmentRequest req)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            assignment.Title = string.IsNullOrWhiteSpace(req.Title) ? assignment.Title : req.Title;
            assignment.Description = req.Description ?? assignment.Description;
            assignment.DueDate = req.DueDate ?? assignment.DueDate;
            assignment.UpdatedAt = DateTime.UtcNow;

            if (req.Attachment != null && req.Attachment.Length > 0)
            {
                assignment.AttachmentUrl = await FileUploadService.SaveFileAsync(req.Attachment);
            }

            await _context.SaveChangesAsync();
            return Ok(assignment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

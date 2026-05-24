using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;

namespace StudentPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentsController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public AssignmentsController(StudentPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _context.Assignments
                .Include(a => a.CreatedBy)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.AttachmentUrl,
                    a.CreatedAt,
                    a.UpdatedAt,
                    CreatedBy = a.CreatedBy == null ? null : new { a.CreatedBy.Id, a.CreatedBy.Name }
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignment(int id)
        {
            var assignment = await _context.Assignments
                .Include(a => a.CreatedBy)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.AttachmentUrl,
                    a.CreatedAt,
                    a.UpdatedAt,
                    CreatedBy = a.CreatedBy == null ? null : new { a.CreatedBy.Id, a.CreatedBy.Name }
                })
                .FirstOrDefaultAsync();

            if (assignment == null) return NotFound();
            return Ok(assignment);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalApi.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace StudentPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StudentPortalContext _context;

        public AuthController(StudentPortalContext context)
        {
            _context = context;
        }

        public class SignupDto
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto req)
        {
            if (await _context.Users.AnyAsync(u => u.Email == req.Email))
            {
                return BadRequest("Email already in use.");
            }

            var user = new User
            {
                Name = req.Name,
                Email = req.Email,
                PasswordHash = req.Password,
                Role = "student"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Name, user.Email, user.ProfilePictureUrl, user.Role });
        }

        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto req)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email && u.PasswordHash == req.Password);
            
            if (user == null) return Unauthorized("Invalid email or password.");

            return Ok(new { user.Id, user.Name, user.Email, user.ProfilePictureUrl, user.Role });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] int userId, [FromForm] string? password, [FromForm] IFormFile? profilePicture)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = password;
            }

            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
                var filepath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = $"/uploads/{fileName}";
            }

            await _context.SaveChangesAsync();
            return Ok(new { user.Id, user.Name, user.Email, user.ProfilePictureUrl, user.Role });
        }
    }
}

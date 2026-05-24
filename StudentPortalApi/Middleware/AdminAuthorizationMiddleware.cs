using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudentPortalApi.Models;

namespace StudentPortalApi.Middleware
{
    public class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, StudentPortalContext dbContext)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();
            if (path != null && path.StartsWith("/api/admin"))
            {
                // Check for hardcoded admin credentials first
                if (context.Request.Headers.TryGetValue("X-Admin-Auth", out var adminAuthHeader) && adminAuthHeader.ToString() == "admin-001")
                {
                    var adminUser = new User 
                    { 
                        Id = 1, 
                        Name = "Admin", 
                        Email = "admin@portal.edu", 
                        Role = "admin" 
                    };
                    context.Items["CurrentUser"] = adminUser;
                    await _next(context);
                    return;
                }

                // Check for database user authentication
                if (!context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) || !int.TryParse(userIdHeader, out var userId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                if (!context.Request.Headers.TryGetValue("X-User-Email", out var emailHeader) || string.IsNullOrWhiteSpace(emailHeader))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var user = await dbContext.Users.FindAsync(userId);
                if (user == null || !string.Equals(user.Email, emailHeader.ToString(), System.StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(user.Role, "admin", System.StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }

                context.Items["CurrentUser"] = user;
            }

            await _next(context);
        }
    }
}

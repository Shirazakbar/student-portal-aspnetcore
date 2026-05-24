using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using StudentPortalApi.Models;

namespace StudentPortalApi.Attributes
{
    // Note: This attribute is now mainly for defense-in-depth.
    // The middleware (AdminAuthorizationMiddleware) does the primary auth work.
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // The middleware already validates and sets CurrentUser in Items
            // This attribute provides defense-in-depth by double-checking
            var currentUser = context.HttpContext.Items["CurrentUser"] as User;
            if (currentUser == null || !string.Equals(currentUser.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReadersConnect.Domain.Models.Identity;

namespace ReadersConnect.Web.Extensions.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;
        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = (ApplicationUser?)context.HttpContext.Items["User"];
            if (user == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            // Retrieve UserManager from the DI container
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();

            if (userManager == null)
            {
                context.Result = new JsonResult(new { message = "UserManager not found" }) { StatusCode = StatusCodes.Status500InternalServerError };
                return;
            }

            // Retrieve roles for the user
            var userRoles = await userManager.GetRolesAsync(user);

            // Check if the user has at least one of the required roles
            if (_roles.Length > 0 && !_roles.Any(role => userRoles.Contains(role)))
            {
                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}
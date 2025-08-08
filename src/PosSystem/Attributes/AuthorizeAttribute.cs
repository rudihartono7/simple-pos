using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace PosSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[]? _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // Check if user is authenticated via JWT
            var user = context.HttpContext.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                // Not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            // Role-based authorization
            if (_roles != null && _roles.Length > 0)
            {
                var userRole = user.FindFirst("Role")?.Value ?? user.FindFirst("role")?.Value;
                if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole))
                {
                    // User doesn't have required role
                    context.Result = new JsonResult(new { message = "Forbidden - Insufficient permissions" }) { StatusCode = StatusCodes.Status403Forbidden };
                    return;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
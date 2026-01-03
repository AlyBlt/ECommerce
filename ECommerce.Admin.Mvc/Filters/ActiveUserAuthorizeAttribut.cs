using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace ECommerce.Admin.Mvc.Filters
{
    public class ActiveUserAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity!.IsAuthenticated)
                return;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            var userService = context.HttpContext
                .RequestServices
                .GetRequiredService<IUserService>();

            var userEntity = userService
                .GetAsync(int.Parse(userIdClaim.Value))
                .GetAwaiter()
                .GetResult();

            if (userEntity == null || !userEntity.Enabled)
            {
                context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
                    .GetAwaiter()
                    .GetResult();

                
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Auth",
                    new { passive = true });
            }
        }
    }
}

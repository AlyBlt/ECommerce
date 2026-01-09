using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection; 
using System.Security.Claims;

namespace ECommerce.Application.Filters
{
    public class ActiveUserAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // [AllowAnonymous] işaretli aksiyonlarda kontrol yapma
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
                return;

            // Kullanıcı giriş yapmamışsa zaten AuthorizeAttribute onu Login'e atacak, biz karışmayalım
            if (user.Identity == null || !user.Identity.IsAuthenticated)
                return;

            
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return;

           

            if (user.IsInRole("SystemAdmin"))
                return;

            // Service provider üzerinden UserService'i alalım
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

            try
            {
                // Kullanıcıyı API'den çekiyoruz
                var userResponse = await userService.GetCurrentUserAsync(int.Parse(userIdClaim.Value), false, false);

                // Eğer kullanıcı silindiyse veya pasif yapıldıysa
                if (userResponse == null || !userResponse.Enabled)
                {
                    // Oturumu sonlandır (Cookie'yi sil)
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    // Session temizle
                   // context.HttpContext.Session.Clear();

                    // Login sayfasına yönlendir ve nedenini bildir
                    context.Result = new RedirectToActionResult("Login", "Auth", new { passive = true });
                    return;
                }
            }
            catch
            {
                // API o an kapalıysa veya hata verirse kullanıcıyı atmayalım, devam etsin.
                return;
            }

        }
    }
}
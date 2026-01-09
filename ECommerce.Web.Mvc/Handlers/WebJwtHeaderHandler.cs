using System.Net.Http.Headers;

namespace ECommerce.Web.Mvc.Handlers
{
    public class WebJwtHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebJwtHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Sadece Web projesine özel çerez ismini okur
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["Web_JwtToken"];

            if (string.IsNullOrEmpty(token))
            {
                token = _httpContextAccessor.HttpContext?.Items["jwtToken"] as string;
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

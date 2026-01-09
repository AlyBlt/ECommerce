using System.Net.Http.Headers;


namespace ECommerce.Admin.Mvc.Handlers
{
    public class AdminJwtHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminJwtHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Sadece Admin projesine özel çerez ismini okur
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["Admin_JwtToken"];

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

using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Web.Mvc.Handlers;
using ECommerce.Web.Mvc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// JWT içindeki claim isimlerinin otomatik deðiþtirilmesini engeller
//Kýsa isimleri (ClaimTypes.Role yerine -- role, sub--id için gibi) kullanmak için denenebilir.
//Ýleride Python, Node.js, Go gibi dillerle birleþtirmek istersek bakalým. --->>
//System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddTransient<WebJwtHeaderHandler>();
// DATAAPI ÝLE HABERLEÞME (HttpClientFactory)
builder.Services.AddHttpClient("DataApi", client =>
{
    // Öncelik appsettings.json'daki URL, yoksa default localhost
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://localhost:7278/");
})
.AddHttpMessageHandler<WebJwtHeaderHandler>();

//FILEAPI
builder.Services.AddHttpClient<FileApiService>(client =>
{
    // appsettings.json'dan FileApiUrl'i oku, yoksa fallback olarak localhost:7207 kullan
    client.BaseAddress = new Uri(builder.Configuration["FileApiUrl"] ?? "https://localhost:7207/");
})
.AddHttpMessageHandler<WebJwtHeaderHandler>();  //dosya indirme herkese açýktýr ama ürün yükleme yetki gerektirir.

// JSON ayarlarýný global olarak sisteme tanýt-küçük büyük harf durumu
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddAuthorization(options =>
{
    // Daha güvenli (harf duyarsýz) tanýmlama
    options.AddPolicy("BuyerOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.Claims.Any(c =>
                (c.Type == ClaimTypes.Role || c.Type == "role") &&
                c.Value.Equals("Buyer", StringComparison.OrdinalIgnoreCase))));

    options.AddPolicy("SellerOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.Claims.Any(c =>
                (c.Type == ClaimTypes.Role || c.Type == "role") &&
                c.Value.Equals("Seller", StringComparison.OrdinalIgnoreCase))));

});
// Cookie Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/login";
    options.Cookie.Name = "WebAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Strict yerine Lax daha uyumludur
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTP/HTTPS uyumu

});
   
builder.Services.AddScoped<IProductService, ProductApiService>();
builder.Services.AddScoped<ICategoryService, CategoryApiService>();
builder.Services.AddScoped<ICartService, CartApiService>();
builder.Services.AddScoped<IOrderService, OrderApiService>();
builder.Services.AddScoped<IFavoriteService, FavoriteApiService>();
builder.Services.AddScoped<IUserService, UserApiService>();
builder.Services.AddScoped<IRoleService, RoleApiService>();
builder.Services.AddScoped<IEmailService, EmailService>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();


//Session Ayarlarý
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "ECommerce.Web.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


//  Pipeline Sýralamasý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// ImageHelper'ý konfigürasyon ile besle
var configuration = app.Services.GetRequiredService<IConfiguration>();
ECommerce.Web.Mvc.Helpers.ImageHelper.Initialize(configuration);
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Authentication'dan önce gelmeli
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
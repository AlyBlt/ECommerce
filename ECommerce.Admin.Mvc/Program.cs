using ECommerce.Admin.Mvc.Handlers;
using ECommerce.Admin.Mvc.Services;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//Kýsa isimleri (ClaimTypes.Role yerine -- role, sub--id için gibi) kullanmak için denenebilir.
//Ýleride Python, Node.js, Go gibi dillerle birleþtirmek istersek bakalým. --->>
//System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// JWT HANDLER KAYDI
builder.Services.AddTransient<AdminJwtHeaderHandler>();

//  API ÝLE HABERLEÞME YAPILANDIRMASI (Tek Bir Kayýt Yeterli)
builder.Services.AddHttpClient("DataApi", client =>
{
    // Öncelik appsettings.json'daki URL, yoksa default localhost
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://localhost:7278/");
})
.AddHttpMessageHandler<AdminJwtHeaderHandler>(); // Otomatik Token ekleyiciyi buraya baðladýk

//FILEAPI
builder.Services.AddHttpClient<FileApiService>(client =>
{
    // appsettings.json'dan FileApiUrl'i oku, yoksa fallback olarak localhost:7207 kullan
    client.BaseAddress = new Uri(builder.Configuration["FileApiUrl"] ?? "https://localhost:7207/");
})
.AddHttpMessageHandler<AdminJwtHeaderHandler>();  //dosya indirme herkese açýktýr ama ürün yükleme yetki gerektirir.


// JSON ayarlarýný global olarak sisteme tanýtalým
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddAuthorization(options =>
{
   
    options.AddPolicy("AdminPanelAccess", policy =>
     policy.RequireAssertion(context =>
         context.User.Claims.Any(c =>
             (c.Type == ClaimTypes.Role || c.Type == "role") &&
             (c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
              c.Value.Equals("SystemAdmin", StringComparison.OrdinalIgnoreCase))
         )
     ));
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
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/Login";
    options.Cookie.Name = "AdminAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Strict yerine Lax daha uyumludur
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // HTTP/HTTPS uyumu
    options.Events = new CookieAuthenticationEvents();
       
    });


// API SERVICE KAYITLARI (Interface - Somut Sýnýf Eþleþmeleri)
builder.Services.AddScoped<IProductService, ProductApiService>();
builder.Services.AddScoped<ICategoryService, CategoryApiService>();
builder.Services.AddScoped<IProductCommentService, ProductCommentApiService>();
builder.Services.AddScoped<IRoleService, RoleApiService>();
builder.Services.AddScoped<IUserService, UserApiService>();
builder.Services.AddScoped<IDashboardService, DashboardApiService>();
builder.Services.AddScoped<IOrderService, OrderApiService>();



builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware Sýralamasý (Sýra çok önemlidir!)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// ImageHelper'ý konfigürasyon ile besle
var configuration = app.Services.GetRequiredService<IConfiguration>();
ECommerce.Admin.Mvc.Helpers.ImageHelper.Initialize(configuration);
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Önce kimlik doðrula
app.UseAuthorization();  // Sonra yetki kontrol et

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
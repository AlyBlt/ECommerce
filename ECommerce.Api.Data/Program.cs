using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Data.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//Kýsa isimleri (ClaimTypes.Role yerine -- role, sub--id için gibi) kullanmak için denenebilir.
//Ýleride Python, Node.js, Go gibi dillerle birleþtirmek istersek bakalým. --->>
//System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Veritabaný ve Mevcut Servislerini Buraya Kaydediyoruz (DI)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDataLayer(connectionString);

// Servis Kayýtlarý
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductCommentService, ProductCommentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<JWTService>(); // JWTService'i buraya da ekledik
builder.Services.AddScoped<IEmailService, EmailService>(); //EmailService


// JWT Ayarlarýný Yapýlandýrýyoruz
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            RoleClaimType = ClaimTypes.Role,
            
        };
        
    });

builder.Services.AddAuthorization(options =>
{
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

    options.AddPolicy("AdminPanelAccess", policy =>
    policy.RequireAssertion(context =>
        context.User.Claims.Any(c =>
            (c.Type == ClaimTypes.Role || c.Type == "role") &&
            (c.Value.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
             c.Value.Equals("SystemAdmin", StringComparison.OrdinalIgnoreCase))
        )
    ));
});

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // API genellikle veriyi CamelCase gönderir, bunu da sabitleyebilirsin:
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


//yeni--seeddata için----------------
using (var scope = app.Services.CreateScope())
{
    // extension metodu burada kullanýyoruz
    // Bu metod hem EnsureCreated() yapacak hem de Seed verilerini basacak
    scope.ServiceProvider.ApplySeedData();
}
//-------------------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Önemli: Authorization'dan önce gelmeli
app.UseAuthorization();

app.MapControllers();

app.Run();

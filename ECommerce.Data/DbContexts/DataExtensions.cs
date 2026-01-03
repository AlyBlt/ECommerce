using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.DbContexts
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ECommerceDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductCommentRepository, ProductCommentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();


            return services;
        }

        // Yeni: Seed metodunu dışarı açan extension
        public static void ApplySeedData(this IServiceProvider services)
        {
            SeedData.Seed(services); // Burası internal sınıfı görebilir
        }
    }
}

using ECommerce.Application.Interfaces;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;

public class DashboardService : IDashboardService
{
    private readonly ECommerceDbContext _db;

    public DashboardService(ECommerceDbContext db)
    {
        _db = db;
    }

    public DashboardStatsEntity GetDashboardStats()
    {
        return new DashboardStatsEntity
        {
            TotalUsers = _db.Users.Count(),
            TotalProducts = _db.Products.Count(),
            TotalCategories = _db.Categories.Count(),
            PendingComments = _db.ProductComments.Count(pc => !pc.IsConfirmed),
            PendingSellers = _db.Users.Count(u => !u.IsSellerApproved),
            DailySales = 0 // Eğer satış tablosu varsa burayı güncelle
        };
    }
}
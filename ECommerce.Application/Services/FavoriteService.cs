using ECommerce.Application.Interfaces;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ECommerceDbContext _db;

        public FavoriteService(ECommerceDbContext db)
        {
            _db = db;
        }

        public IEnumerable<FavoriteEntity> GetByUser(int userId)
        {
            return _db.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToList();
        }

        public void Add(int userId, int productId)
        {
            if (!Exists(userId, productId))
            {
                var favorite = new FavoriteEntity
                {
                    UserId = userId,
                    ProductId = productId
                };
                _db.Favorites.Add(favorite);
                _db.SaveChanges();
            }
        }

        public void Remove(int userId, int productId)
        {
            var favorite = _db.Favorites.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
            if (favorite != null)
            {
                _db.Favorites.Remove(favorite);
                _db.SaveChanges();
            }
        }

        public bool Exists(int userId, int productId)
        {
            return _db.Favorites.Any(f => f.UserId == userId && f.ProductId == productId);
        }
    }
}

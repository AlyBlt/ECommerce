using ECommerce.Application.Interfaces;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ECommerceDbContext _db;

        public CartService(ECommerceDbContext db)
        {
            _db = db;
        }

        public IEnumerable<CartItemEntity> GetCartItems(int userId)
        {
            return _db.CartItems
                      .Include(c => c.Product)
                      .Where(c => c.UserId == userId)
                      .ToList();
        }

        public void AddToCart(int userId, int productId, int quantity)
        {
            var item = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            if (item != null)
            {
                item.Quantity += (byte)quantity;
            }
            else
            {
                _db.CartItems.Add(new CartItemEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = (byte)quantity,
                    CreatedAt = DateTime.Now
                });
            }
            _db.SaveChanges();
        }

        public void UpdateCartItem(int userId, int productId, int quantity)
        {
            var item = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    _db.CartItems.Remove(item);
                else
                    item.Quantity = (byte)quantity;

                _db.SaveChanges();
            }
        }

        public void RemoveCartItem(int userId, int productId)
        {
            var item = _db.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                _db.SaveChanges();
            }
        }

        public void ClearCart(int userId)
        {
            var items = _db.CartItems.Where(c => c.UserId == userId);
            _db.CartItems.RemoveRange(items);
            _db.SaveChanges();
        }
    }
}
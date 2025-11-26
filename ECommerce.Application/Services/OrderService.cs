using ECommerce.Application.Interfaces;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly ECommerceDbContext _db;

        public OrderService(ECommerceDbContext db)
        {
            _db = db;
        }

        public OrderEntity CreateOrder(int userId, string address, string paymentMethod, List<CartItemEntity> cartItems)
        {
            // Entity: DB’ye kaydedilecek gerçek nesne
            var order = new OrderEntity
            {
                UserId = userId,  // Entity: DB’de UserId kolonuna karşılık
                Address = address,  // Entity: DB’de Address kolonuna karşılık
                OrderCode = $"ORD-{new Random().Next(1000, 9999)}",
                CreatedAt = DateTime.Now
            };

            _db.Orders.Add(order);
            _db.SaveChanges(); // Id oluşsun

            
            foreach (var item in cartItems)
            {
                var product = _db.Products.Find(item.ProductId); // Entity: ProductEntity DB’den çekiliyor
                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,          
                    UnitPrice = product?.Price ?? 0,
                    CreatedAt = DateTime.Now
                };
                _db.OrderItems.Add(orderItem);
            }

            _db.SaveChanges();
            return order;
        }

        public IEnumerable<OrderEntity> GetOrdersByUser(int userId)
        {
            // Entity: DB’den tüm siparişleri getiriyoruz, navigation properties ile OrderItems ve Product da dahil
            return _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToList();
        }

        public OrderEntity? GetOrder(int orderId)
        {
            // Entity: Tek siparişi çekiyoruz, yine navigation property dahil
            return _db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == orderId);
        }
    }
}
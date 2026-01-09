using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Data.Repositories
{
    internal class OrderRepository : BaseRepository<OrderEntity>, IOrderRepository
    {
        public OrderRepository(ECommerceDbContext context) : base(context)
        {
        }

        public async Task<OrderEntity> CreateOrderAsync(OrderEntity order, List<CartItemEntity> cartItems)
        {
            await Context.Orders.AddAsync(order);
            await Context.SaveChangesAsync(); // OrderId oluşması için

            var productIds = cartItems.Select(ci => ci.ProductId).ToList();
            var products = await Context.Products
                                        .Where(p => productIds.Contains(p.Id))
                                        .ToListAsync();

            decimal total = 0;

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                var unitPrice = product.Price;
                total += unitPrice * item.Quantity;

                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    CreatedAt = DateTime.Now
                };

                await Context.OrderItems.AddAsync(orderItem);
            }

            order.TotalPrice = total;
            await Context.SaveChangesAsync();

            return order;
        }

        public async Task<List<OrderEntity>> GetOrdersByUserAsync(int userId)
        {
            return await Context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<OrderEntity?> GetOrderWithDetailsAsync(int orderId)
        {
            return await Context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}

using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;



namespace ECommerce.Data.Repositories
{
    internal class ProductRepository : BaseRepository<ProductEntity>, IProductRepository
    {
        public ProductRepository(ECommerceDbContext context) : base(context)
        {
        }

        // Ürünleri kategori, satıcı, yorumlar ve görseller ile birlikte getir
        public async Task<List<ProductEntity>> GetAllWithIncludesAsync()
        {
            return await GetAllQuery().ToListAsync();
        }

        // Belirli bir ürünü kategori, satıcı, yorumlar ve görseller ile birlikte getir
        public async Task<ProductEntity?> GetByIdWithIncludesAsync(int id)
        {
            return await Context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        //--------------------------------------
        // Aktif ürünleri filtrele
        public async Task<List<ProductEntity>> GetActiveProductsAsync()
        {
            return await GetAllQuery().Where(p => p.Enabled)
                .ToListAsync();
        }


        // Popüler ürünleri filtrele (İndirimli ürünler, Featured ürünler vs.)
        public async Task<List<ProductEntity>> GetFeaturedProductsAsync()
        {
            return await GetAllQuery()
               .Where(p => p.IsFeatured)
               .OrderByDescending(p => p.CreatedAt).Take(5)
               .ToListAsync();
        }
       

        // İndirimli ürünleri filtrele
        public async Task<List<ProductEntity>> GetDiscountedProductsAsync()
        {
            return await GetAllQuery()
              .Where(p => p.OldPrice > p.Price)
              .OrderByDescending(p => p.CreatedAt)
              .Take(5).ToListAsync();
        }
       

        // Yeni gelen ürünleri al
        public async Task<List<ProductEntity>> GetNewArrivalProductsAsync()
        {
            return await GetAllQuery()
              .OrderByDescending(p => p.CreatedAt).Take(5).ToListAsync();
        }
        //------------------------------------------------------------------
       
        // İlgili ürünleri kategoriye göre al
        public async Task<List<ProductEntity>> GetRelatedProductsAsync(int categoryId, int currentProductId)
        {
            return await GetAllQuery()
              .Where(p => p.CategoryId == categoryId && p.Id != currentProductId)
              .OrderByDescending(p => p.CreatedAt)
              .Take(4).ToListAsync();
        }
      
        // Arama için ürünleri getir
        public async Task<List<ProductEntity>> GetProductsForSearchAsync(string query, string? category, byte? rating)
        {
            var queryLower = query.ToLower();

            var productQuery = GetAllQuery()
            .Where(p =>
            p.Name.ToLower().Contains(queryLower) ||
            p.Details.ToLower().Contains(queryLower));

            if (!string.IsNullOrEmpty(category))
            {
                productQuery = productQuery.Where(p => p.Category.Name.ToLower() == category.ToLower());
            }

            // Rating filtresi varsa, ortalama puanla filtrele
            if (rating.HasValue)
            {
                productQuery = productQuery.Where(p => p.Comments.Any(c => c.IsConfirmed) &&
                p.Comments.Where(c => c.IsConfirmed).Average(c => (byte)c.StarCount) >= rating.Value);
            }

            return await productQuery.ToListAsync();
        }

       
        public IQueryable<ProductEntity> GetAllQuery()
        {
            return Context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User);
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            // OrderItems veya OrderDetails tablosu üzerinden kontrol ediyoruz
            return await Context.Set<OrderItemEntity>()
                .AnyAsync(oi => oi.Order.UserId == userId && oi.ProductId == productId);
        }
    }
}

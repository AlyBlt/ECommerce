using ECommerce.Application.DTOs.Product;
using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;


namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        // ------------------- CRUD -------------------
        // ------------------- READ (DTO Dönüşümlü) -------------------
        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _productRepository.GetAllWithIncludesAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDTO?> GetAsync(int id)
        {
            var product = await _productRepository.GetByIdWithIncludesAsync(id);
            return product == null ? null : MapToDto(product);
        }

        // ------------------- WRITE -------------------
        public async Task AddAsync(ProductDTO dto)
        {
            var entity = new ProductEntity
            {
                Name = dto.Name,
                Price = dto.Price,
                OldPrice = dto.OldPrice,
                Details = dto.Details,
                StockAmount = dto.StockAmount,
                CategoryId = dto.CategoryId,
                SellerId = dto.SellerId,
                IsFeatured = dto.IsFeatured,
                Enabled = true,
                CreatedAt = DateTime.Now,
                Images = new List<ProductImageEntity>()
            };

            if (!string.IsNullOrEmpty(dto.MainImageUrl))
            {
                entity.Images.Add(new ProductImageEntity
                {
                    Url = dto.MainImageUrl,
                    IsMain = true
                });
            }

            await _productRepository.AddAsync(entity);
            await _productRepository.SaveAsync();
        }

        public async Task UpdateAsync(ProductDTO dto)
        {
            var entity = await _productRepository.GetByIdWithIncludesAsync(dto.Id);
            if (entity == null) throw new Exception("Product not found");

            // Alanları güncelle
            entity.Name = dto.Name;
            entity.Price = dto.Price;
            entity.OldPrice = dto.OldPrice;
            entity.Details = dto.Details;
            entity.StockAmount = dto.StockAmount;
            entity.CategoryId = dto.CategoryId;
            entity.IsFeatured = dto.IsFeatured;
            entity.Enabled = dto.Enabled;

            // --- RESİM GÜNCELLEME MANTIĞI BURASI ---
            if (!string.IsNullOrEmpty(dto.MainImageUrl))
            {
                // Mevcut bir ana resim var mı?
                var mainImage = entity.Images?.FirstOrDefault(i => i.IsMain);

                if (mainImage != null)
                {
                    // Varsa: URL'ini yeni gelenle değiştir
                    mainImage.Url = dto.MainImageUrl;
                }
                else
                {
                    // Yoksa veya liste boşsa: Yeni bir ImageEntity ekle
                    if (entity.Images == null) entity.Images = new List<ProductImageEntity>();

                    entity.Images.Add(new ProductImageEntity
                    {
                        Url = dto.MainImageUrl,
                        IsMain = true,
                        ProductId = entity.Id
                    });
                }
            }

            await _productRepository.UpdateAsync(entity);
            await _productRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdWithIncludesAsync(id);

            if (product == null)
                throw new ArgumentException("Product not found");

            // Burada ürün ile ilişkili yorumlar ve görseller cascade delete ile otomatik silinir
            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveAsync();
        }


        public async Task ToggleStatusAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                product.Enabled = !product.Enabled;
                await _productRepository.SaveAsync();
            }
        }

        // ------------------- COMMENT -------------------
        public async Task AddCommentAsync(int productId, ProductCommentDTO commentDto)
        {
            // Satın alma kontrolü
            var hasPurchased = await _productRepository.HasUserPurchasedProductAsync(commentDto.UserId, productId);
            if (!hasPurchased)
            {
                throw new InvalidOperationException("You can only comment on products you have purchased.");
            }
            var product = await _productRepository.GetByIdWithIncludesAsync(productId);
            if (product != null)
            {
                var commentEntity = new ProductCommentEntity
                {
                    ProductId = productId,
                    UserId = commentDto.UserId,
                    Text = commentDto.Text,
                    StarCount = commentDto.StarCount,
                    CreatedAt = DateTime.Now,
                    IsConfirmed = false // Admin onayı bekler
                };

                product.Comments.Add(commentEntity);
                await _productRepository.SaveAsync();
            }
        }

        // ------------------- LISTING & SEARCH (DTO Dönüşümlü) -------------------
        public async Task<List<ProductDTO>> GetActiveProductsAsync()
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDTO>> GetFeaturedProductsAsync()
        {
            var products = await _productRepository.GetFeaturedProductsAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDTO>> GetDiscountedProductsAsync()
        {
            var products = await _productRepository.GetDiscountedProductsAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDTO>> GetNewArrivalProductsAsync()
        {
            var products = await _productRepository.GetNewArrivalProductsAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDTO>> GetRelatedProductsAsync(int categoryId, int currentProductId)
        {
            var products = await _productRepository.GetRelatedProductsAsync(categoryId, currentProductId);
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductDTO>> SearchProductsAsync(string query, string? category, byte? rating)
        {
            var products = await _productRepository.GetProductsForSearchAsync(query, category, rating);
            return products.Select(MapToDto).ToList();
        }

        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            // Repository'deki o süper sorguyu çağırıyoruz
            return await _productRepository.HasUserPurchasedProductAsync(userId, productId);
        }

        // ------------------- MAPPING (Merkezi Dönüşüm) -------------------
        private ProductDTO MapToDto(ProductEntity entity)
        {
            // Sadece onaylı yorumları ayıklayalım
            var confirmedComments = entity.Comments?.Where(c => c.IsConfirmed).ToList()
                                    ?? new List<ProductCommentEntity>();
            return new ProductDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                OldPrice = entity.OldPrice,
                Details = entity.Details,
                StockAmount = entity.StockAmount,
                Enabled = entity.Enabled,
                IsFeatured = entity.IsFeatured,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.Name ?? "Uncategorized",
                SellerId = entity.SellerId,
                SellerName = entity.Seller != null ? $"{entity.Seller.FirstName} {entity.Seller.LastName}" : "Unknown Seller",
                CreatedAt = entity.CreatedAt,
                Rating = entity.Rating,
                ReviewCount = confirmedComments.Count,
                MainImageUrl = entity.Images?.FirstOrDefault(i => i.IsMain)?.Url
                               ?? entity.Images?.FirstOrDefault()?.Url
                               ?? "default.jpg",
                // Yorum listesini DTO'ya aktaralım (Details sayfasında görünmesi için)
                Comments = confirmedComments.Select(c => new ProductCommentDTO
                {
                    Id = c.Id,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    CreatedAt = c.CreatedAt,
                    UserName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : "Customer",
                    IsConfirmed = c.IsConfirmed
                }).ToList()
            };
        }


    }
}


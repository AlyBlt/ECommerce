using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Application.DTOs.ProductComment;


namespace ECommerce.Application.Services
{
    public class ProductCommentService : IProductCommentService
    {
        private readonly IProductCommentRepository _productCommentRepository;

        public ProductCommentService(IProductCommentRepository productCommentRepository)
        {
            _productCommentRepository = productCommentRepository;
        }

        // Tüm yorumları getir
        public async Task<IEnumerable<ProductCommentDTO>> GetAllAsync()
        {
            var comments = await _productCommentRepository.GetCommentsWithDetailsAsync();
            return comments.Select(MapToDto).ToList();
        }

        // Belirli bir yorumu getir
        public async Task<ProductCommentDTO?> GetAsync(int id)
        {
            var comment = (await _productCommentRepository.GetCommentsWithDetailsAsync())
                  .FirstOrDefault(c => c.Id == id);

            return comment == null ? null : MapToDto(comment);
        }

        // Yorum ekleme işlemi
        public async Task AddAsync(ProductCommentDTO dto)
        {
            var entity = new ProductCommentEntity
            {
                ProductId = dto.ProductId,
                UserId = dto.UserId,
                Text = dto.Text,
                StarCount = dto.StarCount,
                CreatedAt = DateTime.Now,
                IsConfirmed = false // Yeni yorum onay bekler
            };
            await _productCommentRepository.AddAsync(entity);
            await _productCommentRepository.SaveAsync();
        }

        // Yorum güncelleme işlemi
        public async Task UpdateAsync(ProductCommentDTO dto)
        {
            var entity = await _productCommentRepository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Text = dto.Text;
                entity.StarCount = dto.StarCount;
                // Diğer güncellenebilir alanlar...

                await _productCommentRepository.UpdateAsync(entity);
                await _productCommentRepository.SaveAsync(); 
            }
        }

        // Yorum silme işlemi
        public async Task DeleteAsync(int id)
        {
            var productComment = await _productCommentRepository.GetByIdAsync(id);
            if (productComment != null)
            {
                await _productCommentRepository.DeleteAsync(productComment);    // BaseRepository'den gelen metot
            }
        }


        // Yorum onaylama işlemi
        public async Task ApproveCommentAsync(int id)
        {
            await _productCommentRepository.ApproveCommentAsync(id);  // Repository'den gelen metot
        }


        // Yorum reddetme işlemi
        public async Task RejectCommentAsync(int id)
        {
            await _productCommentRepository.RejectCommentAsync(id);  // Repository'den gelen metot
        }

        // --- MAPPING ---
        private ProductCommentDTO MapToDto(ProductCommentEntity entity)
        {
            return new ProductCommentDTO
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Name ?? "Unknown Product",
                UserId = entity.UserId,
                UserName = entity.User != null ? $"{entity.User.FirstName} {entity.User.LastName}" : "Anonymous",
                Text = entity.Text,
                StarCount = entity.StarCount,
                IsConfirmed = entity.IsConfirmed,
                IsRejected = entity.IsRejected,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
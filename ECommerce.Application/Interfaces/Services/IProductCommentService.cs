using ECommerce.Application.DTOs.ProductComment;
using ECommerce.Domain.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IProductCommentService
    {
        Task<IEnumerable<ProductCommentDTO>> GetAllAsync();
        Task<ProductCommentDTO?> GetAsync(int id);
        Task AddAsync(ProductCommentDTO dto);
        Task UpdateAsync(ProductCommentDTO dto);
        Task DeleteAsync(int id);
        Task ApproveCommentAsync(int id);
        Task RejectCommentAsync(int id);
    }
}
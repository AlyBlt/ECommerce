using ECommerce.Application.DTOs.Category;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetAsync(int id);
        Task AddAsync(CategoryDTO catDto);
        Task UpdateAsync(CategoryDTO catDto);
        Task DeleteAsync(int id);
        Task<bool> HasProductsAsync(int categoryId);
    }
}

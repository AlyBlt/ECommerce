using ECommerce.Application.DTOs.Category;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;


namespace ECommerce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBaseRepository<ProductEntity> _productRepository;

        public CategoryService(ICategoryRepository categoryRepository, IBaseRepository<ProductEntity> productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        // Get all categories 
        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var entities = await _categoryRepository.GetAllAsync();
            return entities.Select(e => new CategoryDTO
            {
                Id = e.Id,
                Name = e.Name,
                Color = e.Color,
                IconCssClass = e.IconCssClass,
                ImageUrl = e.ImageUrl
            });
        }

        // Get a single category by ID 
        public async Task<CategoryDTO?> GetAsync(int id)
        {
            var entity = await _categoryRepository.GetByIdAsync(id);
            if (entity == null) return null;
            return new CategoryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Color = entity.Color,
                IconCssClass = entity.IconCssClass,
                ImageUrl = entity.ImageUrl
            };
        }

        // Add a new category 
        //DTO -> Entity Manuel Mapping
        public async Task AddAsync(CategoryDTO catDto)
        {
            var entity = new CategoryEntity
            {
                Name = catDto.Name,
                Color = catDto.Color,
                IconCssClass = catDto.IconCssClass,
                ImageUrl = catDto.ImageUrl,
                CreatedAt = DateTime.Now
            };
            await _categoryRepository.AddAsync(entity);
            await _categoryRepository.SaveAsync();
        }

        // Update an existing category
        public async Task UpdateAsync(CategoryDTO catDto)
        {
            var existingEntity = await _categoryRepository.GetByIdAsync(catDto.Id);
            if (existingEntity == null) throw new KeyNotFoundException("Category not found!");

            // Mevcut entity üzerindeki alanları DTO'dan gelenlerle güncelle
            existingEntity.Name = catDto.Name;
            existingEntity.Color = catDto.Color;
            existingEntity.IconCssClass = catDto.IconCssClass;
            existingEntity.ImageUrl = catDto.ImageUrl;

            await _categoryRepository.UpdateAsync(existingEntity);  
            await _categoryRepository.SaveAsync();
        }

        // Delete a category 
        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found!");

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found!");
            }

            bool hasProducts = await _categoryRepository.HasProductsAsync(id);
            if (hasProducts)
            {
                throw new InvalidOperationException("Cannot delete category with products. Remove or reassign products first.");
            }

            await _categoryRepository.DeleteAsync(category); 
            await _categoryRepository.SaveAsync();
        }

        // Check if a category has associated products asynchronously
        public async Task<bool> HasProductsAsync(int categoryId)
        {
            return await _categoryRepository.HasProductsAsync(categoryId);
        }
    }
    
}
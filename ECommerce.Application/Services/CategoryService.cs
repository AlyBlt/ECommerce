using ECommerce.Application.Interfaces;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ECommerceDbContext _db;

        public CategoryService(ECommerceDbContext db)
        {
            _db = db;
        }

        public IEnumerable<CategoryEntity> GetAll()
        {
            return _db.Categories.ToList();
        }

        public CategoryEntity? Get(int id)
        {
            return _db.Categories.FirstOrDefault(c => c.Id == id);
        }

        public void Add(CategoryEntity category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
        }

        public void Update(CategoryEntity category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var category = _db.Categories.Find(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
            }
        }
    }
}
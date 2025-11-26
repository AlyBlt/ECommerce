using ECommerce.Application.Interfaces;
using ECommerce.Application.ViewModels;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ECommerceDbContext _db;

        public ProductService(ECommerceDbContext db)
        {
            _db = db;
        }

        // ------------------- CRUD -------------------
        public IEnumerable<ProductEntity> GetAll() =>
            _db.Products.Include(p => p.Category)
                        .Include(p => p.Seller)
                        .Include(p => p.Comments)
                        .ToList();

        public ProductEntity? Get(int id) =>
            _db.Products.Include(p => p.Category)
                        .Include(p => p.Seller)
                        .Include(p => p.Comments)
                        .FirstOrDefault(p => p.Id == id);

        public void Add(ProductEntity product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
        }

        public void Update(ProductEntity product)
        {
            _db.Products.Update(product);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }

        public void ToggleStatus(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                product.Enabled = !product.Enabled;
                _db.SaveChanges();
            }
        }

        // ------------------- COMMENT -------------------
        public void AddComment(int productId, ProductCommentEntity comment)
        {
            var product = _db.Products.Include(p => p.Comments)
                                      .FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                comment.CreatedAt = DateTime.Now;
                product.Comments ??= new List<ProductCommentEntity>();
                product.Comments.Add(comment);
                _db.SaveChanges();
            }
        }

        // ------------------- FILTER / LISTING -------------------
        public IEnumerable<ProductEntity> GetActiveProducts()
        {
            return _db.Products.Include(p => p.Category)
                               .Include(p => p.Seller)
                               .Include(p => p.Comments)
                               .Where(p => p.Enabled)
                               .ToList();
        }


        // ------------------- FEATURED PRODUCTS -------------------

        // Popüler Ürünleri
        public IEnumerable<ProductEntity> GetPopularProducts()
        {
            return _db.Products.Where(p => p.IsFeatured).OrderByDescending(p => p.CreatedAt).Take(5).ToList();
        }

        // İndirimli Ürünleri
        public IEnumerable<ProductEntity> GetDiscountedProducts()
        {
            return _db.Products.Where(p => p.OldPrice > p.Price).OrderByDescending(p => p.CreatedAt).Take(5).ToList();
        }

        // Yeni Gelen Ürünleri
        public IEnumerable<ProductEntity> GetNewArrivalProducts()
        {
            return _db.Products.OrderByDescending(p => p.CreatedAt).Take(5).ToList();
        }
    }
}

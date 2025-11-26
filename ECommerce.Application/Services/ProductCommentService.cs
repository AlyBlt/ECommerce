using ECommerce.Application.Interfaces;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class ProductCommentService : IProductCommentService
    {
        private readonly ECommerceDbContext _db;

        public ProductCommentService(ECommerceDbContext db)
        {
            _db = db;
        }

        public IEnumerable<ProductCommentEntity> GetAll() =>
            _db.ProductComments.Include(pc => pc.Product).Include(pc => pc.User).ToList();

        public ProductCommentEntity? Get(int id) =>
            _db.ProductComments.Include(pc => pc.Product).Include(pc => pc.User).FirstOrDefault(pc => pc.Id == id);

        public void Add(ProductCommentEntity productComment)
        {
            _db.ProductComments.Add(productComment);
            _db.SaveChanges();
        }

        public void Update(ProductCommentEntity productComment)
        {
            _db.ProductComments.Update(productComment);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var productComment = _db.ProductComments.Find(id);
            if (productComment != null)
            {
                _db.ProductComments.Remove(productComment);
                _db.SaveChanges();
            }
        }

        // Approve metodunu ApproveComment olarak değiştirdik
        public void ApproveComment(int id)
        {
            var comment = _db.ProductComments.Find(id);
            if (comment != null)
            {
                comment.IsConfirmed = true;
                _db.SaveChanges();
            }
        }
    }
}
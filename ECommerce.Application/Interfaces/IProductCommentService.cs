using ECommerce.Data.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces
{
    public interface IProductCommentService
    {
        IEnumerable<ProductCommentEntity> GetAll();
        ProductCommentEntity? Get(int id);
        void Add(ProductCommentEntity productComment);
        void Update(ProductCommentEntity productComment);
        void Delete(int id);
        void ApproveComment(int id);  
    }
}
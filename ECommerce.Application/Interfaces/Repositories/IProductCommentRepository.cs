using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Repositories
{
    public interface IProductCommentRepository : IBaseRepository<ProductCommentEntity>
    {
        Task ApproveCommentAsync(int id);
        Task RejectCommentAsync(int id);
        Task<List<ProductCommentEntity>> GetCommentsWithDetailsAsync();
    }
}

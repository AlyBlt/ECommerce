using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Data.Repositories
{
    internal class ProductCommentRepository : BaseRepository<ProductCommentEntity>, IProductCommentRepository
    {
        public ProductCommentRepository(ECommerceDbContext context) : base(context)
        {
        }

        public async Task<List<ProductCommentEntity>> GetCommentsWithDetailsAsync()
        {
            return await Context.ProductComments
                .Include(c => c.Product)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task ApproveCommentAsync(int id)
        {
            var comment = await Context.ProductComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsConfirmed = true;
                await Context.SaveChangesAsync();
            }
        }

        public async Task RejectCommentAsync(int id)
        {
            var comment = await Context.ProductComments.FindAsync(id);
            if (comment != null)
            {
                comment.IsConfirmed = false;
                comment.IsRejected = true;
                await Context.SaveChangesAsync();
            }
        }
    }
}

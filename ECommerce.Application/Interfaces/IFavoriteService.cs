using ECommerce.Data.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces
{
    public interface IFavoriteService
    {
        IEnumerable<FavoriteEntity> GetByUser(int userId);
        void Add(int userId, int productId);
        void Remove(int userId, int productId);
        bool Exists(int userId, int productId);
    }
}
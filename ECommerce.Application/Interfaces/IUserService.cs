using ECommerce.Data.Entities;
using System.Collections.Generic;

namespace ECommerce.Application.Interfaces
{
    public interface IUserService
    {
        IEnumerable<UserEntity> GetAll();
        UserEntity? Get(int id, bool includeOrders = false, bool includeProducts = false);
        UserEntity? GetByEmail(string email);
        UserEntity Register(string email, string firstName, string lastName, string password);
        void Add(UserEntity user);
        void Update(UserEntity user);
        void Delete(int id);
        void ApproveSeller(int id);

        
    }
}
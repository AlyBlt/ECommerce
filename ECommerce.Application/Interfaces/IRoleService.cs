using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IRoleService
    {
        IEnumerable<RoleEntity> GetAll();
        RoleEntity? Get(int id);
        void Add(RoleEntity role);
        void Update(RoleEntity role);
        void Delete(int id);
    }
}

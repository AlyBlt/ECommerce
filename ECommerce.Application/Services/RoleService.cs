using ECommerce.Application.Interfaces;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly ECommerceDbContext _db;

        public RoleService(ECommerceDbContext db)
        {
            _db = db;
        }

        public IEnumerable<RoleEntity> GetAll() => _db.Roles.ToList();

        public RoleEntity? Get(int id) => _db.Roles.FirstOrDefault(r => r.Id == id);

        public void Add(RoleEntity role)
        {
            _db.Roles.Add(role);
            _db.SaveChanges();
        }

        public void Update(RoleEntity role)
        {
            _db.Roles.Update(role);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var role = _db.Roles.Find(id);
            if (role != null)
            {
                _db.Roles.Remove(role);
                _db.SaveChanges();
            }
        }
    }
}
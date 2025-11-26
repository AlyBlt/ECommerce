using ECommerce.Application.Interfaces;
using ECommerce.Data;
using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ECommerceDbContext _db;

        public UserService(ECommerceDbContext db)
        {
            _db = db;
        }

        public IEnumerable<UserEntity> GetAll() => _db.Users.Include(u => u.Role).ToList();

        public UserEntity? Get(int id, bool includeOrders = false, bool includeProducts = false)
        {
            IQueryable<UserEntity> query = _db.Users.Include(u => u.Role);

            if (includeOrders)
                query = query.Include(u => u.Orders);

            if (includeProducts)
                query = query.Include(u => u.Products);

            return query.FirstOrDefault(u => u.Id == id);
        }

        public void Add(UserEntity user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void Update(UserEntity user)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }

        public void ApproveSeller(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                user.IsSellerApproved = true; // entity alanına göre düzenlenecek
                _db.SaveChanges();
            }
        }

        //Gorev11-Eklenenler
        //-------------------------------------
        public UserEntity? GetByEmail(string email)
        {
            return _db.Users.FirstOrDefault(u => u.Email == email);
        }

        public UserEntity Register(string email, string firstName, string lastName, string password)
        {
            var user = new UserEntity
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Password = password,  // Ödev gereği hash yapmaya gerek yok
                RoleId = 2, // Buyer
                Enabled = true,
                CreatedAt = DateTime.Now,
                IsSellerApproved = false
            };

            _db.Users.Add(user);
            _db.SaveChanges();
            return user;
        }
    }
}
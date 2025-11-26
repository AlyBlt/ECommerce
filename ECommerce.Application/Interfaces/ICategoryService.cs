using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<CategoryEntity> GetAll();
        CategoryEntity? Get(int id);
        void Add(CategoryEntity category);
        void Update(CategoryEntity category);
        void Delete(int id);
    }
}

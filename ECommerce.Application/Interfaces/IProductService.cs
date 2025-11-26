using ECommerce.Application.ViewModels;
using ECommerce.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductEntity> GetAll();
        ProductEntity? Get(int id);
        void Add(ProductEntity product);
        void Update(ProductEntity product);
        void Delete(int id);
        void ToggleStatus(int id);


        //Featured ürünleri alırken sadece Entity döndürüyoruz
        IEnumerable<ProductEntity> GetPopularProducts();
        IEnumerable<ProductEntity> GetDiscountedProducts();
        IEnumerable<ProductEntity> GetNewArrivalProducts();

    }
}

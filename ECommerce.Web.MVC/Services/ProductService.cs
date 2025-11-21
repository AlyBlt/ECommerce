using ECommerceWeb.MVC.Models;

namespace ECommerceWeb.MVC.Services
{
    public class ProductService
    {
        private static List<CategoryViewModel> categories = new()
        {
            new CategoryViewModel{CategoryId=1, CategoryName="Gıda"},
            new CategoryViewModel{CategoryId=2, CategoryName="Meyve"},
            new CategoryViewModel{CategoryId=3, CategoryName="Erkek Giyim"},
            new CategoryViewModel{CategoryId=4, CategoryName="Kadın Giyim"},
            new CategoryViewModel{CategoryId=5, CategoryName="Çocuk Giyim"},
        };

        private static List<ProductViewModel> products = new()
        {
            new ProductViewModel{ProductId=1, ProductName = "Elma", ProductPrice = 50.5m, ProductDescription = "Seben Elması",
                Category =
                new CategoryViewModel{
                    CategoryId=2, CategoryName="Meyve"
                }
            },

            new ProductViewModel{ProductId=2, ProductName = "Makarna", ProductPrice = 32, ProductDescription = "Barilla Makarna",
                Category =
               new CategoryViewModel {
                    CategoryId=1, CategoryName="Gıda"
                }
            },
            new ProductViewModel{ProductId=3, ProductName = "Kadın Sweat", ProductPrice = 50.5m, ProductDescription = "Seben Elması",
                Category =
                new CategoryViewModel{
                    CategoryId=4, CategoryName="Kadın Giyim"
                }
            },
            new ProductViewModel{ProductId=4, ProductName = "Süt", ProductPrice = 45, ProductDescription = "İçim süt ll yüzde 3 yağlı",
                Category =
               new CategoryViewModel {
                    CategoryId=1, CategoryName="Gıda"
                }
            },new ProductViewModel{ProductId=5, ProductName = "Ananas", ProductPrice = 99.90m, ProductDescription = "Güney <amerika ananas",
                Category =
               new CategoryViewModel {
                    CategoryId=1, CategoryName="Gıda"
                }
            },
        };

        /// <summary>
        /// Ürün oluşturma
        /// </summary>
        /// <returns></returns>
        public static ProductViewModel CreateProduct(ProductViewModel productViewModel)
        {
            products.Add(productViewModel);

            return productViewModel;
        }

        /// <summary>
        /// Ürün silme
        /// </summary>
        /// <returns></returns>
        public static void DeleteProduct(int productId)
        {
            // object olarak silme
            // parametre olarak model alır
            //products.Remove(productViewModel);

            // id ile silme
            var product = products.Find(x => x.ProductId == productId);

            if (product != null)
            {
                products.Remove(product);
            }
        }

        /// <summary>
        /// Ürün düzenleme
        /// </summary>
        /// <returns></returns>
        public static void EditProduct(int productId, ProductViewModel newProduct)
        {
            // ürünü bul
            var product = products.Find(x => x.ProductId == productId);

            if (product != null)
            {
                products.Remove(product);

                product.ProductPrice = newProduct.ProductPrice;
                product.ProductDescription = newProduct.ProductDescription;
                product.Category = newProduct.Category;
                product.ProductId = newProduct.ProductId;
                product.ProductName = newProduct.ProductName;
                product.ProductComment = newProduct.ProductComment;
                product.ProductStar = newProduct.ProductStar;

                products.Add(product);
            }
        }

        /// <summary>
        /// Ürüne yorum yapma
        /// </summary>
        /// <param name="comment"></param>
        public static void MakeProductComment(int productId, string comment)
        {
            var product = products.Find(i => i.ProductId == productId);

            if (product != null)
            {
                product.ProductComment = comment;
                products.Add(product);

            }
        }

        /// <summary>
        /// Ürüne yıldız verme
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="star"></param>
        public static void GiveProductStar(int productId, short star)
        {
            var product = products.Find(i => i.ProductId == productId);

            if (product != null && (star >= 1 && star <= 5))
            {
                product.ProductStar = star;
                products.Add(product);
            }
        }

        /// <summary>
        /// Ürün listeleme
        /// </summary>
        /// <returns></returns>
        public static List<ProductViewModel> GetAllProducts()
        {
            return products;
        }

        /// <summary>
        /// Kategori listeleme
        /// </summary>
        /// <returns></returns>
        public static List<CategoryViewModel> GetAllCategories()
        {
            return categories;
        }
    }
}

using ECommerce.Data.DbContexts;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ECommerce.Data
{
    internal static class SeedData
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

                // Veritabanı yoksa oluşturur
                context.Database.EnsureCreated();

                // Mevcut Initialize metodunu çalıştırır
                Initialize(context);
            }
        }
        public static void Initialize(ECommerceDbContext context)
        {
            var fixedDate = new DateTime(2025, 1, 1);

            // 1) ROLES
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new RoleEntity { Name = "Admin", CreatedAt = fixedDate },
                    new RoleEntity { Name = "Seller", CreatedAt = fixedDate },
                    new RoleEntity { Name = "Buyer", CreatedAt = fixedDate }
                );

                context.SaveChanges();
            }

            // 2) SAMPLE USERS
            if (!context.Users.Any())
            {
                var adminRoleId = context.Roles.First(r => r.Name == "Admin").Id;
                var sellerRoleId = context.Roles.First(r => r.Name == "Seller").Id;
                var buyerRoleId = context.Roles.First(r => r.Name == "Buyer").Id;

                context.Users.AddRange(
                    new UserEntity
                    {
                        Email = "admin1@site.com",
                        FirstName = "Admin1",
                        LastName = "User",
                        Password = "123",        // Ödev gereği düz şifre
                        RoleId = adminRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "New York, USA",
                        Phone = "123-123-12-12",
                       
                        
                    },
                    new UserEntity
                    {
                        Email = "admin2@site.com",
                        FirstName = "Admin2",
                        LastName = "User",
                        Password = "123",        
                        RoleId = adminRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Wisconsin, USA",
                        Phone = "135-135-15-15",


                    },
                    new UserEntity
                    {
                        Email = "seller1@site.com",
                        FirstName = "Alice",
                        LastName = "Seller",
                        Password = "123",
                        RoleId = sellerRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Los Angeles, USA",
                        Phone = "234-234-23-23",
                        IsSellerApproved = true,
                       
                    },
                    new UserEntity
                    {
                        Email = "seller2@site.com",
                        FirstName = "Bob",
                        LastName = "Seller",
                        Password = "123",
                        RoleId = sellerRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Chicago, USA",
                        Phone = "345-345-34-34",
                        IsSellerApproved = true,
                        
                    },
                    new UserEntity
                    {
                        Email = "seller3@site.com",
                        FirstName = "Emily",
                        LastName = "Seller",
                        Password = "123",
                        RoleId = sellerRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Florida, USA",
                        Phone = "344-344-33-33",
                        IsSellerApproved = true,
                        
                    },
                    new UserEntity
                    {
                        Email = "buyer1@site.com",
                        FirstName = "Charlie",
                        LastName = "Buyer",
                        Password = "123",
                        RoleId = buyerRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Houston, USA",
                        Phone = "456-456-45-45",
                    },
                    new UserEntity
                    {
                        Email = "buyer2@site.com",
                        FirstName = "Diana",
                        LastName = "Buyer",
                        Password = "123",
                        RoleId = buyerRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Miami, USA",
                        Phone = "567-567-56-56",
                    },
                     new UserEntity
                     {
                         Email = "buyer3@site.com",
                         FirstName = "Jack",
                         LastName = "Buyer",
                         Password = "123",
                         RoleId = buyerRoleId,
                         Enabled = true,
                         CreatedAt = fixedDate,
                         Address = "Philadelphia, USA",
                         Phone = "566-566-54-54",
                         IsSellerApproved = false,
                         HasPendingSellerRequest = true
                     }
                );

                context.SaveChanges();
            }


            // 3)  CATEGORIES
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new CategoryEntity { Name = "Fruits", Color = "FF0000", IconCssClass = "fa-apple-alt", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Vegetables", Color = "00FF00", IconCssClass = "fa-carrot", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Dairy", Color = "FFFF00", IconCssClass = "fa-cheese", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Bakery", Color = "FFA500", IconCssClass = "fa-bread-slice", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Beverages", Color = "00FFFF", IconCssClass = "fa-wine-bottle", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Snacks", Color = "800080", IconCssClass = "fa-cookie", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Frozen", Color = "0000FF", IconCssClass = "fa-snowflake", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Organic", Color = "008000", IconCssClass = "fa-leaf", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Meat & Fish", Color = "FF00FF", IconCssClass = "fa-drumstick-bite", CreatedAt = fixedDate },
                    new CategoryEntity { Name = "Condiments", Color = "000080", IconCssClass = "fa-pepper-hot", CreatedAt = fixedDate }
                );

                context.SaveChanges();
            }


            // 4)  PRODUCTS
            if (!context.Products.Any())
            {
                var fruitsCategoryId = context.Categories.First(c => c.Name == "Fruits").Id;
                var vegetablesCategoryId = context.Categories.First(c => c.Name == "Vegetables").Id;
                var dairyCategoryId = context.Categories.First(c => c.Name == "Dairy").Id;
                var bakeryCategoryId = context.Categories.First(c => c.Name == "Bakery").Id;
                var beveragesCategoryId = context.Categories.First(c => c.Name == "Beverages").Id;
                var snacksCategoryId = context.Categories.First(c => c.Name == "Snacks").Id;
                var frozenCategoryId = context.Categories.First(c => c.Name == "Frozen").Id;
                var organicCategoryId = context.Categories.First(c => c.Name == "Organic").Id;
                var meatFishCategoryId = context.Categories.First(c => c.Name == "Meat & Fish").Id;
                var condimentsCategoryId = context.Categories.First(c => c.Name == "Condiments").Id;

               
                var seller1Id = context.Users.First(u => u.Email == "seller1@site.com").Id;
                var seller2Id = context.Users.First(u => u.Email == "seller2@site.com").Id;

                context.Products.AddRange(
                    new ProductEntity { Name = "Apple", Price = 3.5m, StockAmount = 50, CategoryId = fruitsCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2023, 1, 1), Enabled = true, Details = "Fresh red apples", IsFeatured = true },
                    new ProductEntity { Name = "Banana", Price = 4m, StockAmount = 60, CategoryId = fruitsCategoryId, SellerId = seller1Id, CreatedAt = fixedDate, Enabled = true, Details = "Sweet bananas", OldPrice=5m },
                    new ProductEntity { Name = "Watermelon", Price = 2.5m, StockAmount = 40, CategoryId = fruitsCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2025, 12, 17), Enabled = true, Details = "Fresh Watermelon" },
                    new ProductEntity { Name = "Grape", Price = 5m, StockAmount = 30, CategoryId = fruitsCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2024, 10, 8), Enabled = true, Details = "Organic Grape", OldPrice = 6m },
                    new ProductEntity { Name = "Carrot", Price = 2m, StockAmount = 20, CategoryId = vegetablesCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2017, 11, 23), Enabled = true, Details = "Organic Carrot", OldPrice = 3m },
                    new ProductEntity { Name = "Bell Pepper", Price = 3m, StockAmount = 80, CategoryId = vegetablesCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2025, 11, 27), Enabled = true, Details = "Fresh Bell Pepper" },
                    new ProductEntity { Name = "Strawberry", Price = 8m, StockAmount = 25, CategoryId = fruitsCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2015, 4, 18), Enabled = true, Details = "Fresh Strawberry", IsFeatured = true },
                    new ProductEntity { Name = "Chicken Egg", Price = 5m, StockAmount = 100, CategoryId = dairyCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2025, 12, 16), Enabled = true, Details = "Chicken Egg", OldPrice=6m },
                    new ProductEntity { Name = "Croissant", Price = 2.5m, StockAmount = 40, CategoryId = bakeryCategoryId, SellerId = seller1Id, CreatedAt = new DateTime(2016, 2, 3), Enabled = true, Details = "Fresh Croissant" },
                    new ProductEntity { Name = "Orange Juice", Price = 3m, StockAmount = 50, CategoryId = beveragesCategoryId, SellerId = seller2Id, CreatedAt = new DateTime(2023, 4, 30), Enabled = true, Details = "Fresh Orange Juice", IsFeatured = true },
                    new ProductEntity { Name = "Chocolate Cookie", Price = 1.5m, StockAmount = 70, CategoryId = snacksCategoryId, SellerId = seller2Id, CreatedAt = new DateTime(2025, 9, 15), Enabled = true, Details = "Tasty Cookie", OldPrice=2m },
                    new ProductEntity { Name = "Frozen Peas", Price = 2m, StockAmount = 30, CategoryId = frozenCategoryId, SellerId = seller2Id, CreatedAt = new DateTime(2024, 5, 16), Enabled = true, Details = "Frozen Peas", OldPrice=3m },
                    new ProductEntity { Name = "Honey", Price = 6m, StockAmount = 25, CategoryId = organicCategoryId, SellerId = seller2Id, CreatedAt = new DateTime(2025, 12, 18), Enabled = true, Details = "Pure Organic Honey", IsFeatured = true },
                    new ProductEntity { Name = "Chicken Breast", Price = 7m, StockAmount = 30, CategoryId = meatFishCategoryId, SellerId = seller2Id, CreatedAt = new DateTime(2022, 7, 8), Enabled = true, Details = "Fresh Chicken Breast", IsFeatured = true },
                    new ProductEntity { Name = "Hot Sauce", Price = 4m, StockAmount = 60, CategoryId = condimentsCategoryId, SellerId = seller2Id, CreatedAt = new DateTime(2024, 7, 2), Enabled = true, Details = "Spicy Hot Sauce", OldPrice = 6m }

                );

                context.SaveChanges();

                var products = context.Products.ToDictionary(p => p.Name);

               // PRODUCT IMAGES
                context.ProductImages.AddRange(
                    new ProductImageEntity
                    {
                        ProductId = products["Apple"].Id,
                        Url = "~/img/product/product-8.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Banana"].Id,
                        Url = "~/img/product/product-2.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Watermelon"].Id,
                        Url = "~/img/product/product-7.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Grape"].Id,
                        Url = "~/img/product/product-4.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Carrot"].Id,
                        Url = "~/img/product/product-15.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Bell Pepper"].Id,
                        Url = "~/img/product/product-14.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Strawberry"].Id,
                        Url = "~/img/product/product-13.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Chicken Egg"].Id,
                        Url = "~/img/product/product-16.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Croissant"].Id,
                        Url = "~/img/product/product-17.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Orange Juice"].Id,
                        Url = "~/img/product/product-18.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Chocolate Cookie"].Id,
                        Url = "~/img/product/product-19.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Frozen Peas"].Id,
                        Url = "~/img/product/product-20.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                   new ProductImageEntity
                    {
                       ProductId = products["Honey"].Id,
                       Url = "~/img/product/product-21.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Chicken Breast"].Id,
                        Url = "~/img/product/product-22.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    },
                    new ProductImageEntity
                    {
                        ProductId = products["Hot Sauce"].Id,
                        Url = "~/img/product/product-23.jpg",
                        CreatedAt = fixedDate,
                        IsMain = true
                    }

              
                );

                context.SaveChanges();
            }




            var buyers = context.Users
                   .Where(u => u.RoleId == context.Roles.First(r => r.Name == "Buyer").Id)
                   .OrderBy(u => u.Id) // ya da başka bir mantıklı sıralama
                   .ToList();

            // 5) ADD COMMENTS (Yorumlar Eklemek)
            if (buyers.Any() && !context.ProductComments.Any())
            {
                var firstProductId = context.Products.First().Id;
                var secondProductId = context.Products.Skip(1).First().Id;
                var thirdProductId = context.Products.Skip(2).First().Id;
                var fourthProductId = context.Products.Skip(3).First().Id;
                var fifthProductId = context.Products.Skip(4).First().Id;
                var sixthProductId = context.Products.Skip(5).First().Id;
                var seventhProductId = context.Products.Skip(6).First().Id;
                var eighthProductId = context.Products.Skip(7).First().Id;
                var ninthProductId = context.Products.Skip(8).First().Id;
                var tenthProductId = context.Products.Skip(9).First().Id;

            
                var firstUserId = buyers[0].Id;  // Charlie
                var secondUserId = buyers[1].Id; // Diana
                var thirdUserId = buyers[2].Id;  // Jack

                context.ProductComments.AddRange(
                    new ProductCommentEntity
                    {
                        ProductId = firstProductId,
                        UserId = firstUserId,
                        StarCount = 5, // Çok beğenilen ürün için yüksek puan
                        Text = "Great product, very fresh!",
                        CreatedAt = fixedDate.AddDays(2),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = firstProductId,
                        UserId = thirdUserId,
                        StarCount = 4, 
                        Text = "High quality, very fresh!",
                        CreatedAt = fixedDate.AddDays(100),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = firstProductId,
                        UserId = secondUserId,
                        StarCount = 4, // Çok beğenilen ürün için yüksek puan
                        Text = "Very tasty!",
                        CreatedAt = fixedDate.AddDays(2),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = secondProductId,
                        UserId = firstUserId,
                        StarCount = 4, // Oldukça beğenilen ama belki küçük bir eksikliği olan ürün
                        Text = "Tasty and sweet, loved it!",
                        CreatedAt = fixedDate.AddDays(60),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = secondProductId,
                        UserId = secondUserId,
                        StarCount = 2, // Oldukça beğenilen ama belki küçük bir eksikliği olan ürün
                        Text = "It wasn't fresh!",
                        CreatedAt = fixedDate.AddDays(30),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = thirdProductId,
                        UserId = secondUserId,
                        StarCount = 3, // Ortalama puan, ne çok beğenilmiş ne de çok kötü
                        Text = "Not bad, but could be better.",
                        CreatedAt = fixedDate.AddDays(4),
                        IsConfirmed = true
                    },

                     new ProductCommentEntity
                     {
                         ProductId = thirdProductId,
                         UserId = thirdUserId,
                         StarCount = 3, // Ortalama puan, ne çok beğenilmiş ne de çok kötü
                         Text = "Not good enough!",
                         CreatedAt = fixedDate.AddDays(40),
                         IsConfirmed = true
                     },

                    new ProductCommentEntity
                    {
                        ProductId = fourthProductId,
                        UserId = secondUserId,
                        StarCount = 2, // Zayıf beğenilen ürün, belki kalitesiz
                        Text = "Very disappointed.",
                        CreatedAt = fixedDate.AddDays(5),
                        IsConfirmed = false
                    },
                    new ProductCommentEntity
                    {
                        ProductId = fifthProductId,
                        UserId = thirdUserId,
                        StarCount = 5, // Harika beğenilen ürün
                        Text = "Excellent quality and flavor, will buy again!",
                        CreatedAt = fixedDate.AddDays(6),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = sixthProductId,
                        UserId = thirdUserId,
                        StarCount = 4, // Beğenilen ama ufak eksiklikler olabilir
                        Text = "Good taste, but a little expensive.",
                        CreatedAt = fixedDate.AddDays(7),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = seventhProductId,
                        UserId = firstUserId,
                        StarCount = 3, // Ortalama bir ürün
                        Text = "It's okay, but I expected more.",
                        CreatedAt = fixedDate.AddDays(8),
                        IsConfirmed = true
                    },
                     new ProductCommentEntity
                     {
                         ProductId = seventhProductId,
                         UserId = thirdUserId,
                         StarCount = 1, // Ortalama bir ürün
                         Text = "Very bad! I couldn't even eat it!",
                         CreatedAt = fixedDate.AddDays(80),
                         IsConfirmed = true
                     },
                    new ProductCommentEntity
                    {
                        ProductId = eighthProductId,
                        UserId = secondUserId,
                        StarCount = 1, // Hiç beğenilmeyen bir ürün
                        Text = "Did not like it at all, very disappointing.",
                        CreatedAt = fixedDate.AddDays(9),
                        IsConfirmed = false
                    },
                    new ProductCommentEntity
                    {
                        ProductId = ninthProductId,
                        UserId = thirdUserId,
                        StarCount = 4, // Beğenilen ama küçük bir eleştiri
                        Text = "Good product, but the packaging could be improved.",
                        CreatedAt = fixedDate.AddDays(10),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = ninthProductId,
                        UserId = secondUserId,
                        StarCount = 5, // Beğenilen ama küçük bir eleştiri
                        Text = "Great product, packaging was perfect!",
                        CreatedAt = fixedDate.AddDays(18),
                        IsConfirmed = true
                    },
                    new ProductCommentEntity
                    {
                        ProductId = tenthProductId,
                        UserId = firstUserId,
                        StarCount = 5, // Mükemmel ürün
                        Text = "Amazing taste, absolutely loved it!",
                        CreatedAt = fixedDate.AddDays(11),
                        IsConfirmed = true
                    },
                     new ProductCommentEntity
                     {
                         ProductId = tenthProductId,
                         UserId = secondUserId,
                         StarCount = 5, // Mükemmel ürün
                         Text = "Really loved it!",
                         CreatedAt = fixedDate.AddDays(11),
                         IsConfirmed = true
                     }
                );

                context.SaveChanges();
            }
        }
    }
}
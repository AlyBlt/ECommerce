using ECommerce.Data.DbContexts;
using ECommerce.Data.Entities;
using System;
using System.Linq;

namespace ECommerce.Data
{
    public static class SeedData
    {
        public static void Initialize(ECommerceDbContext context)
        {
            context.Database.EnsureCreated();

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
                        Email = "admin@site.com",
                        FirstName = "Admin",
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
                        Email = "seller1@site.com",
                        FirstName = "Alice",
                        LastName = "Seller",
                        Password = "123",
                        RoleId = sellerRoleId,
                        Enabled = true,
                        CreatedAt = fixedDate,
                        Address = "Los Angeles, USA",
                        Phone = "234-234-23-23",
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

                var sellerId = context.Users.First(u => u.RoleId == context.Roles.First(r => r.Name == "Admin").Id).Id;

                context.Products.AddRange(
                    new ProductEntity { Name = "Apple", Price = 3.5m, StockAmount = 50, CategoryId = fruitsCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh red apples", ImageUrl= "~/img/product/product-8.jpg", IsFeatured = true },
                    new ProductEntity { Name = "Banana", Price = 4m, StockAmount = 60, CategoryId = fruitsCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Sweet bananas", ImageUrl = "~/img/product/product-2.jpg", OldPrice=5m },
                    new ProductEntity { Name = "Watermelon", Price = 2.5m, StockAmount = 40, CategoryId = fruitsCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh Watermelon", ImageUrl = "~/img/product/product-7.jpg" },
                    new ProductEntity { Name = "Grape", Price = 5m, StockAmount = 30, CategoryId = fruitsCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Organic Grape", ImageUrl = "~/img/product/product-4.jpg", OldPrice = 6m },
                    new ProductEntity { Name = "Carrot", Price = 2m, StockAmount = 20, CategoryId = vegetablesCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Organic Carrot", ImageUrl = "~/img/product/product-15.jpg", OldPrice = 3m },
                    new ProductEntity { Name = "Bell Pepper", Price = 3m, StockAmount = 80, CategoryId = vegetablesCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh Bell Pepper", ImageUrl = "~/img/product/product-14.jpg" },
                    new ProductEntity { Name = "Strawberry", Price = 8m, StockAmount = 25, CategoryId = fruitsCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh Strawberry", ImageUrl = "~/img/product/product-13.jpg", IsFeatured = true },
                    new ProductEntity { Name = "Chicken Egg", Price = 5m, StockAmount = 100, CategoryId = dairyCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Chicken Egg", ImageUrl = "~/img/product/product-16.jpg", OldPrice=6m },
                    new ProductEntity { Name = "Croissant", Price = 2.5m, StockAmount = 40, CategoryId = bakeryCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh Croissant", ImageUrl = "~/img/product/product-17.jpg" },
                    new ProductEntity { Name = "Orange Juice", Price = 3m, StockAmount = 50, CategoryId = beveragesCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh Orange Juice", ImageUrl = "~/img/product/product-18.jpg" },
                    new ProductEntity { Name = "Chocolate Cookie", Price = 1.5m, StockAmount = 70, CategoryId = snacksCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Tasty Cookie", ImageUrl = "~/img/product/product-19.jpg", OldPrice=2m },
                    new ProductEntity { Name = "Frozen Peas", Price = 2m, StockAmount = 30, CategoryId = frozenCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Frozen Peas", ImageUrl = "~/img/product/product-20.jpg", OldPrice=3m },
                    new ProductEntity { Name = "Honey", Price = 6m, StockAmount = 25, CategoryId = organicCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Pure Organic Honey", ImageUrl = "~/img/product/product-21.jpg", IsFeatured = true },
                    new ProductEntity { Name = "Chicken Breast", Price = 7m, StockAmount = 30, CategoryId = meatFishCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Fresh Chicken Breast", ImageUrl = "~/img/product/product-22.jpg", IsFeatured = true },
                    new ProductEntity { Name = "Hot Sauce", Price = 4m, StockAmount = 60, CategoryId = condimentsCategoryId, SellerId = sellerId, CreatedAt = fixedDate, Enabled = true, Details = "Spicy Hot Sauce", ImageUrl = "~/img/product/product-23.jpg", OldPrice = 6m }

                );

                context.SaveChanges();
            }



            // 5) ADD COMMENTS (Yorumlar Eklemek)
            if (!context.ProductComments.Any())
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

                var firstUserId = context.Users.First(u => u.RoleId == context.Roles.First(r => r.Name == "Buyer").Id).Id;
                var secondUserId = context.Users.Skip(1).First(u => u.RoleId == context.Roles.First(r => r.Name == "Buyer").Id).Id;
                var thirdUserId = context.Users.Skip(2).First(u => u.RoleId == context.Roles.First(r => r.Name == "Buyer").Id).Id;

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
                        ProductId = secondProductId,
                        UserId = firstUserId,
                        StarCount = 4, // Oldukça beğenilen ama belki küçük bir eksikliği olan ürün
                        Text = "Tasty and sweet, loved it!",
                        CreatedAt = fixedDate.AddDays(3),
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
                        ProductId = fourthProductId,
                        UserId = secondUserId,
                        StarCount = 2, // Zayıf beğenilen ürün, belki kalitesiz
                        Text = "The quality is not great, very disappointed.",
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
                        ProductId = tenthProductId,
                        UserId = firstUserId,
                        StarCount = 5, // Mükemmel ürün
                        Text = "Amazing taste, absolutely loved it!",
                        CreatedAt = fixedDate.AddDays(11),
                        IsConfirmed = true
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
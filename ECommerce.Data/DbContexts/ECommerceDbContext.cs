using ECommerce.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ECommerce.Data.DbContexts

{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<FavoriteEntity> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //UserEntity

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Password)
                    .IsRequired();

                entity.Property(u => u.Enabled)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .IsRequired();
            });
            //RoleEntity

            modelBuilder.Entity<RoleEntity>(entity =>
            {
                entity.HasKey(r => r.Id); // PK
                entity.Property(r => r.Name)
                      .IsRequired()
                      .HasMaxLength(10); // Max 10 karakter
                entity.Property(r => r.CreatedAt)
                      .IsRequired();
            });
            //ProductImageEntity

            modelBuilder.Entity<ProductImageEntity>(entity =>
            {
                entity.HasKey(pi => pi.Id); // PK
                entity.Property(pi => pi.Id)
                      .ValueGeneratedOnAdd(); // Identity

                entity.Property(pi => pi.ProductId)
                      .IsRequired();

                entity.Property(pi => pi.Url)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(pi => pi.CreatedAt)
                      .IsRequired();

                // FK relation
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.Images)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ProductEntity configuration
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(p => p.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Details)
                    .HasMaxLength(1000);

                entity.Property(p => p.StockAmount)
                    .IsRequired();

                entity.Property(p => p.Enabled)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(p => p.CreatedAt)
                    .IsRequired();

                // Add IsFeatured property configuration
                entity.Property(p => p.IsFeatured)
                    .IsRequired()
                    .HasDefaultValue(false); // Varsayılan değer olarak false

                // Relations
                entity.HasOne(p => p.Seller)
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.SellerId)
                    .OnDelete(DeleteBehavior.Restrict); // Cascade path hatasını önler

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Comments)
                    .WithOne(c => c.Product)
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Images)
                    .WithOne(i => i.Product)
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //ProductCommentEntity

            modelBuilder.Entity<ProductCommentEntity>(entity =>
            {
                entity.HasKey(pc => pc.Id);

                entity.Property(pc => pc.Text)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(pc => pc.StarCount)
                    .IsRequired();

                entity.Property(pc => pc.IsConfirmed)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(pc => pc.CreatedAt)
                    .IsRequired();

                // Relations
                entity.HasOne(pc => pc.Product)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(pc => pc.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // Multiple cascade paths önlemek için

                entity.HasOne(pc => pc.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(pc => pc.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //CategoryEntity
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);


                entity.Property(c => c.Color)
                    .IsRequired()
                    .HasMaxLength(6);


                entity.Property(c => c.IconCssClass)
                    .IsRequired()
                    .HasMaxLength(50);


                entity.Property(c => c.CreatedAt)
                    .IsRequired();

                // Relation
                entity.HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict); // Multiple cascade paths önlemek için
            });

            modelBuilder.Entity<ProductCommentEntity>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Text)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(c => c.StarCount)
                      .IsRequired();

                entity.Property(c => c.IsConfirmed)
                      .IsRequired()
                      .HasDefaultValue(false);

                entity.Property(c => c.CreatedAt)
                      .IsRequired();

                // FK relation
                entity.HasOne(c => c.Product)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


        }
    }

}

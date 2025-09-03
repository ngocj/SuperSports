using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;
using SP.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Context
{
    public class SPContext : DbContext
    {
        public SPContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }      
        public DbSet<Employee> Employees { get; set; }
        public DbSet<FeedBack> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Image> Images { get; set; }


        public DbSet<Ward> Wards { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Province> Provinces { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVariantConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new DiscountConfiguration());
            modelBuilder.ApplyConfiguration(new ImageConfiguration());
            modelBuilder.ApplyConfiguration(new WardConfiguration());
            modelBuilder.ApplyConfiguration(new DistrictConfiguration());
            modelBuilder.ApplyConfiguration(new ProvinceConfiguration());
            modelBuilder.ApplyConfiguration(new SubCategoryConfiguration());

                         
            // brand
            var defaultDate = new DateTime(2024, 01, 01);

            modelBuilder.Entity<Brand>().HasData(
      new Brand { Id = 1, BrandName = "Nike", Description = "Thương hiệu thể thao nổi tiếng", CreatedAt = defaultDate },
      new Brand { Id = 2, BrandName = "Adidas", Description = "Thương hiệu đến từ Đức", CreatedAt = defaultDate },
      new Brand { Id = 3, BrandName = "Puma", Description = "Nổi bật với thiết kế năng động", CreatedAt = defaultDate },
      new Brand { Id = 4, BrandName = "Under Armour", Description = "Chuyên đồ thể thao hiệu suất cao", CreatedAt = defaultDate },
      new Brand { Id = 5, BrandName = "New Balance", Description = "Nổi bật với giày chạy bộ", CreatedAt = defaultDate },
      new Brand { Id = 6, BrandName = "Asics", Description = "Chuyên giày chạy bộ và thể thao", CreatedAt = defaultDate },
      new Brand { Id = 7, BrandName = "Reebok", Description = "Thương hiệu thể thao đa dạng", CreatedAt = defaultDate }
  );

            // category
            modelBuilder.Entity<Category>().HasData(
    new Category { Id = 1, CategoryName = "Nam", Description = "Trang phục thể thao nam", CreatedAt = defaultDate },
    new Category { Id = 2, CategoryName = "Nữ", Description = "Trang phục thể thao nữ", CreatedAt = defaultDate },
    new Category { Id = 3, CategoryName = "Trẻ em", Description = "Trang phục thể thao trẻ em", CreatedAt = defaultDate },
    new Category { Id = 4, CategoryName = "Phụ kiện", Description = "Túi, nón, tất thể thao", CreatedAt = defaultDate }
);



            // add subcategory
            modelBuilder.Entity<SubCategory>().HasData(
    // Subcategories cho "Nam" (CategoryId = 1)
    new SubCategory { Id = 1, Name = "Áo thun nam", CategoryId = 1 },
    new SubCategory { Id = 2, Name = "Quần nam", CategoryId = 1 },
    new SubCategory { Id = 3, Name = "Giày nam", CategoryId = 1 },

    // Subcategories cho "Nữ" (CategoryId = 2)
    new SubCategory { Id = 4, Name = "Áo thun nữ", CategoryId = 2 },
    new SubCategory { Id = 5, Name = "Đầm", CategoryId = 2 },
    new SubCategory { Id = 6, Name = "Giày nữ", CategoryId = 2 },

    // Subcategories cho "Trẻ em" (CategoryId = 3)
    new SubCategory { Id = 7, Name = "Áo trẻ em", CategoryId = 3 },
    new SubCategory { Id = 8, Name = "Quần trẻ em", CategoryId = 3 },
    new SubCategory { Id = 9, Name = "Giày trẻ em", CategoryId = 3 },

    // Subcategories cho "Phụ kiện" (CategoryId = 4)
    new SubCategory { Id = 10, Name = "Túi", CategoryId = 4 },
    new SubCategory { Id = 11, Name = "Nón", CategoryId = 4 },
    new SubCategory { Id = 12, Name = "Tất", CategoryId = 4 }
);


            // discsount
            modelBuilder.Entity<Discount>().HasData(
                new Discount
                {
                    Id = 1,
                    Name = "Khuyến mãi khai trương",    
                    Percent = 10,
                    Description = "Giảm 10% mừng khai trương",
                    IsActive = true,
                    DateStart = defaultDate,
                    DateEnd = defaultDate.AddMonths(1)
                },
                new Discount
                {
                    Id = 2,
                    Name = "Giảm giá mùa hè",
                    Percent = 15,
                    Description = "Ưu đãi mùa hè",
                    IsActive = true,
                    DateStart = defaultDate.AddDays(5),
                    DateEnd = defaultDate.AddMonths(2)
                },
                new Discount
                {
                    Id = 3,
                    Name = "Giảm giá cuối tuần",
                    Percent = 20,
                    Description = "Giảm giá sốc cuối tuần",
                    IsActive = true,
                    DateStart = defaultDate.AddDays(10),
                    DateEnd = defaultDate.AddMonths(1).AddDays(10)
                },
                new Discount
                {
                    Id = 4,
                    Name = "Khuyến mãi cho khách hàng thân thiết",
                    Percent = 5,
                    Description = "Giảm nhẹ cho khách quen",
                    IsActive = true,
                    DateStart = defaultDate.AddDays(3),
                    DateEnd = defaultDate.AddMonths(3)
                },
                new Discount
                {
                    Id = 5,
                    Name = "Giảm giá Black Friday",
                    Percent = 25,
                    Description = "Black Friday Sale",
                    IsActive = true,
                    DateStart = defaultDate.AddDays(1),
                    DateEnd = defaultDate.AddMonths(1).AddDays(5)
                }
            );
        }


    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
           
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.ProductName).IsUnique();

            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);

            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(p => p.Discount)
                   .WithMany(d => d.Products)
                   .HasForeignKey(p => p.DiscountId)
                   .OnDelete(DeleteBehavior.SetNull); 

            builder.HasOne(p => p.Brand)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SubCategory)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.SubCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);


        }
    }
    
}

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
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariant");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            
            builder.Property(p => p.Price).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

            builder.Property(p => p.Quantity).IsRequired();

            builder.Property(p => p.Color).IsRequired();

            builder.Property(p => p.Size).IsRequired();

            builder.HasIndex(p => new { p.ProductId, p.Color, p.Size })
            .IsUnique();

            builder.Property(p => p.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasOne(p => p.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(p => p.ProductId);

        }
    }
    
    
}

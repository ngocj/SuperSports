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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
            builder.HasIndex(c => c.CategoryName).IsUnique();

            builder.Property(c => c.Description).HasMaxLength(500);

            builder.Property(c => c.IsActive).HasDefaultValue(true).IsRequired();

            builder.Property(c => c.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");


        }
    }
    
  
}

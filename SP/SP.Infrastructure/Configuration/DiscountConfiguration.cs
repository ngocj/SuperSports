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
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discount");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Description).HasMaxLength(500);

            builder.Property(x => x.Percent).IsRequired();
            builder.Property(x => x.Percent)
                .HasColumnType("int");
         
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
   
            builder.Property(x => x.DateStart).IsRequired();
            builder.Property(x => x.DateStart)
                .HasColumnType("datetime2");

            builder.Property(x => x.DateEnd).IsRequired();
            builder.Property(x => x.DateEnd)
                .HasColumnType("datetime2");

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

        }
    }
    
}

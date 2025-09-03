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
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Image");


            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedOnAdd();

            builder.Property(i => i.FileName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.FileData)
                .IsRequired();  

            builder.Property(i => i.ContentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(i => new { i.ProductVariantId, i.FileName })
                .IsUnique();

            builder.Property(i => i.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(i => i.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(i => i.ProductVariant)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductVariantId);
        }
    }
    
    
}

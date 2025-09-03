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
    public class FeedbackConfiguration : IEntityTypeConfiguration<FeedBack>
    {
        public void Configure(EntityTypeBuilder<FeedBack> builder)
        {
            builder.ToTable("Feedback");

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).ValueGeneratedOnAdd();

            builder.Property(f => f.Comment).HasMaxLength(500);

            builder.Property(f => f.Rating).IsRequired().HasColumnType("int");

            builder.Property(f => f.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(f => f.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(f => f.User)
                .WithMany(u => u.FeedBacks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.OrderDetail)
           .WithMany(opv => opv.FeedBacks)
           .HasForeignKey(f => new { f.OrderId, f.ProductVariantId })
           .OnDelete(DeleteBehavior.Cascade);


        }
    }
    
}

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
    public class DistrictConfiguration : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable("District");
            builder.HasKey(d => d.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(d => d.Province)
                   .WithMany(p => p.Districts)
                   .HasForeignKey(d => d.ProvinceId);
            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
        }
    }
}

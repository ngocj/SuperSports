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
    public class WardConfiguration : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable("Ward");
            builder.HasKey(w => w.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(w => w.WardName).IsRequired().HasMaxLength(100);

            builder.HasOne(w => w.District)
                   .WithMany(d => d.Wards)
                   .HasForeignKey(w => w.DistrictId);

            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");
           
        }
    }
}

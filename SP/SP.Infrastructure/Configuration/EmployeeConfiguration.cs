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
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.Password).IsRequired().HasMaxLength(100);         

            builder.Property(e => e.PhoneNumber).HasMaxLength(15);
            builder.HasIndex(e => e.PhoneNumber).IsUnique();

            builder.Property(e => e.AddressDetail).HasMaxLength(200);

            builder.Property(e => e.RoleId).HasDefaultValue(3);

            builder.Property(e => e.DateOfBirth)
                   .HasColumnType("date");
            builder.Property(e => e.IsActive).HasDefaultValue(true);

         
            builder.Property(e => e.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime2")
                     .HasDefaultValueSql("GETDATE()");
           
            builder.HasOne(e => e.Role)
                   .WithMany(r => r.Employees)
                   .HasForeignKey(e => e.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
       
            builder.HasOne(e => e.Ward)
                   .WithMany(w => w.Employees)
                   .HasForeignKey(e => e.WardId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

        
        }
    }
    

}

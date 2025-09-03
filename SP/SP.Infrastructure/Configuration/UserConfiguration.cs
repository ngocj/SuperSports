using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SP.Domain.Entity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
 
        builder.ToTable(nameof(User));

        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(x => x.Email).IsUnique();
            
        builder.Property(x => x.Password)
            .HasMaxLength(100);

        builder.Property(x => x.RoleId).HasDefaultValue(4);


        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);
        builder.HasIndex(x => x.PhoneNumber).IsUnique();

        builder.Property(x => x.DateOfBirth)
            .HasColumnType("date");

        builder.Property(x => x.AddressDetail)
            .HasMaxLength(50);

        builder.Property(x => x.IsActive).HasDefaultValue(true);    

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(x => x.Role)
            .WithMany( x => x.Users)   // Một Role có thể liên kết với nhiều User
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Ward)
            .WithMany(x => x.Users)   // Một Ward có thể liên kết với nhiều User
            .HasForeignKey(x => x.WardId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

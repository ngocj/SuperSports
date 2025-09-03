using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Cart");

        builder.HasKey(c => new { c.UserId, c.ProductVariantId });

        builder.Property(c => c.Quantity).IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId);

        builder.HasOne(c => c.ProductVariant)
            .WithMany(pv => pv.Carts)
            .HasForeignKey(c => c.ProductVariantId);
    }
}

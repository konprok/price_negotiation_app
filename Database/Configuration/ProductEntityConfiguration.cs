using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Configuration;

public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable("product_entity");

        builder.HasKey(p => p.Id)
               .HasName("PK_product_entity");

        builder.Property(p => p.Id)
               .HasColumnName("product_id");

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(255) 
               .HasColumnName("name");

        builder.Property(p => p.Description)
               .HasMaxLength(1000)
               .HasColumnName("description");

        builder.Property(p => p.BasePrice)
               .HasPrecision(18, 2)
               .IsRequired()
               .HasColumnName("base_price");

        builder.Property(p => p.CreatedAt)
               .IsRequired()
               .HasColumnName("created_at");

        builder.Property(p => p.ModifiedAt)
               .HasColumnName("modified_at");

        builder.Property(p => p.OwnerId)
               .HasColumnName("owner_id");


        builder.HasOne(p => p.CreatedBy)
               .WithMany(u => u.CreatedProducts)
               .HasForeignKey(p => p.OwnerId)
               .OnDelete(DeleteBehavior.Restrict) 
               .HasConstraintName("FK_product_entity_user");
    }
}

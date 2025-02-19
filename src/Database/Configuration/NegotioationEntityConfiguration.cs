using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Configuration;

public sealed class NegotiationEntityConfiguration : IEntityTypeConfiguration<NegotiationEntity>
{
    public void Configure(EntityTypeBuilder<NegotiationEntity> builder)
    {
        builder.ToTable("negotiation_entity");

        builder.HasKey(n => n.Id)
            .HasName("PK_negotiation_entity");

        builder.Property(n => n.Id)
            .HasColumnName("negotiation_id");

        builder.Property(n => n.ProductId)
            .HasColumnName("product_id");

        builder.Property(n => n.OwnerId)
            .HasColumnName("owner_id");

        builder.Property(n => n.ClientId)
            .HasColumnName("client_id")
            .IsRequired();

        builder.Property(n => n.Finished)
            .HasColumnName("finished");

        builder.Property(n => n.FinalPrice)
            .HasPrecision(18, 2)
            .HasColumnName("final_price");

        builder.Property(n => n.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(n => n.ModyfiedAt)
            .HasColumnName("modified_at");

        builder.HasOne(n => n.Product)
            .WithMany(p => p.Negotiations!)
            .HasForeignKey(n => n.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_negotiation_entity_product");
    }
}
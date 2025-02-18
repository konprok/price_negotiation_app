using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Configuration;

public class PropositionEntityConfiguration : IEntityTypeConfiguration<PropositionEntity>
{
    public void Configure(EntityTypeBuilder<PropositionEntity> builder)
    {
        builder.ToTable("proposition_entity");

        builder.HasKey(p => p.Id)
               .HasName("PK_proposition_entity");

        builder.Property(p => p.Id)
               .HasColumnName("proposition_id");

        builder.Property(p => p.NegotiationId)
               .HasColumnName("negotiation_id");

        builder.Property(p => p.ProposedPrice)
               .HasPrecision(18, 2)
               .HasColumnName("proposed_price");

        builder.Property(p => p.ProposedAt)
               .HasColumnName("proposed_at");

        builder.Property(p => p.IsAccepted)
               .HasColumnName("is_accepted");

        builder.Property(p => p.DecidedAt)
               .HasColumnName("decided_at");

        builder.HasOne(prop => prop.Negotiation)
               .WithMany(n => n.Proposition)
               .HasForeignKey(prop => prop.NegotiationId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_proposition_entity_negotiation");
    }
}

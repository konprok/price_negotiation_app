using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Configuration;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("user_entity");

        builder.HasKey(x => x.Id)
               .HasName("PK_user_entity");

        builder.Property(x => x.Id)
               .HasColumnName("user_id");

        builder.Property(x => x.UserName)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnName("user_name");

        builder.Property(x => x.PasswordHash)
               .IsRequired()
               .HasMaxLength(512)
               .HasColumnName("password_hash");

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(255)
               .HasColumnName("email");

        builder.HasMany(u => u.CreatedProducts)
            .WithOne(p => p.CreatedBy)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Products_CreatedById");

        builder.HasMany(u => u.Negotiations)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Negotiations_UserId");
    }
}

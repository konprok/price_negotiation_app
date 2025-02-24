using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Database.Configuration;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Database.DbContext;

public sealed class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;
    public DbSet<NegotiationEntity> Negotiations { get; set; } = null!;
    public DbSet<PropositionEntity> Propositions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new NegotiationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PropositionEntityConfiguration());
    }
}
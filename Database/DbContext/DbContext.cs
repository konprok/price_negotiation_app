using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Database.Configuration;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Database.DbContext;

public class UserDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly IPasswordHasher _passwordHasher;

    public UserDbContext(DbContextOptions<UserDbContext> options, IPasswordHasher passwordHasher) : base(options)
    {
        _passwordHasher = passwordHasher;
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
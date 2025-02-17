namespace PriceNegotiationApp.Database.Entities;

public sealed class UserEntity
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }

    public ICollection<ProductEntity> CreatedProducts { get; set; } 
        = new List<ProductEntity>();

    public ICollection<NegotiationEntity> Negotiations { get; set; } 
        = new List<NegotiationEntity>();
}
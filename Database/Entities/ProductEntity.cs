namespace PriceNegotiationApp.Database.Entities;

public class ProductEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid OwnerId { get; set; }
    public UserEntity CreatedBy { get; set; } = null!;
    public ICollection<NegotiationEntity>? Negotiations { get; set; }
}

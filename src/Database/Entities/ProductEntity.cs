using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Database.Entities;

public sealed class ProductEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid OwnerId { get; set; }
    public ICollection<NegotiationEntity>? Negotiations { get; set; }

    public ProductEntity()
    {
    }

    public ProductEntity(Product product)
    {
        Name = product.Name;
        Description = product.Description;
        BasePrice = product.BasePrice;
        CreatedAt = DateTime.UtcNow;
    }
}
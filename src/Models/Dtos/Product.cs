using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Models.Dtos;

public class Product
{
    public Product()
    {
    }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
}
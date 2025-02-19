namespace PriceNegotiationApp.Models.Dtos;
public sealed class Product
{
    public Product()
    {
    }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
}
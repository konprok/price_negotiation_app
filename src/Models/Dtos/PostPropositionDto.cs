namespace PriceNegotiationApp.Models.Dtos;

public sealed class PostPropositionDto
{
    public Guid ClientId { get; set; }
    public long ProductId { get; set; }
    public decimal Price { get; set; }
}
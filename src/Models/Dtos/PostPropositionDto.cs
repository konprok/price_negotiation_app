namespace PriceNegotiationApp.Models.Dtos;

public class PostPropositionDto
{
    public Guid ClientId { get; set; }
    public long ProductId { get; set; }
    public decimal Price { get; set; }
}
namespace PriceNegotiationApp.Database.Entities;

public class PropositionEntity
{
    public long Id { get; set; }
    public long NegotiationId { get; set; }
    public decimal ProposedPrice { get; set; }
    public DateTimeOffset ProposedAt { get; set; }
    public bool? Decision { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
    public NegotiationEntity Negotiation { get; set; } = null!;
}
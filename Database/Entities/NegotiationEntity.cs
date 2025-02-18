namespace PriceNegotiationApp.Database.Entities;


public class NegotiationEntity
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ClientId { get; set; }
    public bool Finished { get; set; }
    public decimal? FinalPrice { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModyfiedAt { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public IList<PropositionEntity> Proposition { get; set; } = new List<PropositionEntity>();
}
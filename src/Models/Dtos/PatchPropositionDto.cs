namespace PriceNegotiationApp.Models.Dtos;

public sealed class PatchPropositionDto
{
    public Guid UserId { get; set; }
    public long NegotiationId { get; set; }
    public bool Response { get; set; }
}
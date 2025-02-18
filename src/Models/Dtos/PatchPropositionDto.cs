namespace PriceNegotiationApp.Models.Dtos;

public class PatchPropositionDto
{
    public Guid UserId { get; set; }
    public long NegotiationId { get; set; }
    public bool Response { get; set; }
}
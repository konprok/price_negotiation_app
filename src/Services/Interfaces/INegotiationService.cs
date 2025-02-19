using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Services.Interfaces;

public interface INegotiationService
{
    Task<NegotiationEntity> GetNegotiation(Guid clientId, long productId);
    Task<IEnumerable<NegotiationEntity?>> GetNegotiations(Guid userId);
    Task<PropositionEntity> PostProposition(Guid clientId, long productId, decimal price);
    Task<NegotiationEntity> PatchProposition(Guid userId, long negotiationId, bool response);
}
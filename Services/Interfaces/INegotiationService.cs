using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Services.Interfaces;

public interface INegotiationService
{
    Task<NegotiationEntity> PostNegotiation(Guid clientId, long productId);
    Task<NegotiationEntity> GetNegotiation(Guid clientId, long productId);
    Task<PropositionEntity> PostProposition(long negotiationId, decimal price);
}
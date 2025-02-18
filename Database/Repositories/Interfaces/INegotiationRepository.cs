using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Repositories.Interfaces;

public interface INegotiationRepository
{
    Task<NegotiationEntity?> GetNegotiation(Guid clientId, long productId);
    Task<NegotiationEntity?> GetNegotiation(long negotiationId);
    Task<IEnumerable<NegotiationEntity?>> GetNegotiationsByOwnerId(Guid userId);
    Task<IEnumerable<NegotiationEntity?>> GetNegotiationsByClientId(Guid userId);
    Task InsertNegotiationAsync(NegotiationEntity negotiationEntity);
    Task SaveAsync();
}
using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Repositories.Interfaces;

public interface INegotiationRepository
{
    Task<NegotiationEntity?> GetNegotiation(Guid clientId, long productId);
    Task<NegotiationEntity?> GetNegotiation(long negotiationId);
    Task<IEnumerable<NegotiationEntity?>> GetNegotiations(Guid userId);
    Task InsertNegotiationAsync(NegotiationEntity negotiationEntity);
    Task SaveAsync();
}
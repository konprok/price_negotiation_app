using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Repositories.Interfaces;

public interface INegotiationRepository
{
    Task<NegotiationEntity?> GetNegotiation(Guid clientId, long productId);
    Task<NegotiationEntity?> GetNegotiation(long negotiationId);
    Task InsertProductAsync(NegotiationEntity negotiationEntity);
    Task SaveAsync();
}
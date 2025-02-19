using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Repositories.Interfaces;

public interface IPropositionRepository
{
    Task<PropositionEntity?> GetProposition(long propositionId);
    Task<PropositionEntity?> GetProposition(Guid ownerId, long negotiationId);
    Task InsertPropositionAsync(PropositionEntity propositionEntity);
    Task SaveAsync();
}
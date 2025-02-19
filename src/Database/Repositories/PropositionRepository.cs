using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Database.DbContext;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;

namespace PriceNegotiationApp.Database.Repositories;

public sealed class PropositionRepository : IPropositionRepository
{
    private readonly AppDbContext _dbContext;

    public PropositionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PropositionEntity?> GetProposition(long propositionId)
    {
        return await _dbContext.Propositions
            .FirstOrDefaultAsync(p => p.Id == propositionId);
    }

    public async Task<PropositionEntity?> GetProposition(Guid ownerId, long negotiationId)
    {
        return await _dbContext.Propositions
            .Include(p => p.Negotiation)
            .Where(p => p.NegotiationId == negotiationId
                        && p.Negotiation.OwnerId == ownerId)
            .FirstOrDefaultAsync();
    }

    public async Task InsertPropositionAsync(PropositionEntity propositionEntity)
    {
        await _dbContext.Propositions.AddAsync(propositionEntity);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
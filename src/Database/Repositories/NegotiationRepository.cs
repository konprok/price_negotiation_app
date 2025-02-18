using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Database.DbContext;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;

namespace PriceNegotiationApp.Database.Repositories;

public class NegotiationRepository : INegotiationRepository
{
    private readonly AppDbContext _dbContext;

    public NegotiationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NegotiationEntity?> GetNegotiation(Guid clientId, long productId)
    {
        return await _dbContext.Negotiations
            .Include(x => x.Proposition)
            .Where(x => x.ClientId == clientId && x.ProductId == productId)
            .SingleOrDefaultAsync();
    }

    public async Task<NegotiationEntity?> GetNegotiation(long negotiationId)
    {
        return await _dbContext.Negotiations
            .Include(x => x.Proposition)
            .SingleOrDefaultAsync(x=> x.Id == negotiationId);
    }
    
    public async Task InsertNegotiationAsync(NegotiationEntity negotiationEntity)
    {
        await _dbContext.Negotiations.AddAsync(negotiationEntity);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<NegotiationEntity?>> GetNegotiationsByOwnerId(Guid userId)
    {
        return await _dbContext.Negotiations
            .Include(x => x.Proposition)
            .Where(x => x.OwnerId == userId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<NegotiationEntity?>> GetNegotiationsByClientId(Guid userId)
    {
        return await _dbContext.Negotiations
            .Include(x => x.Proposition)
            .Where(x => x.ClientId == userId)
            .ToListAsync();
    }
}
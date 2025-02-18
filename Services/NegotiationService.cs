using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Services;

public class NegotiationService : INegotiationService
{
    private readonly INegotiationRepository _negotiationRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPropositionRepository _propositionRepository;

    public NegotiationService(INegotiationRepository negotiationRepository, IProductRepository productRepository,
        IPropositionRepository propositionRepository)
    {
        _negotiationRepository = negotiationRepository;
        _productRepository = productRepository;
        _propositionRepository = propositionRepository;
    }

    public async Task<NegotiationEntity> PostNegotiation(Guid clientId, long productId)
    {
        var negotiation = await _negotiationRepository.GetNegotiation(clientId, productId);
        if (negotiation != null)
        {
            throw new NegotiationAlreadyExistException();
        }

        var productEntity = await _productRepository.GetProduct(productId);
        var negotiationEntity = new NegotiationEntity
        {
            ProductId = productId,
            OwnerId = productEntity.OwnerId,
            ClientId = clientId,
            Finished = false,
            CreatedAt = DateTime.UtcNow
        };

        await _negotiationRepository.InsertProductAsync(negotiationEntity);
        await _negotiationRepository.SaveAsync();

        return negotiationEntity;
    }

    public async Task<NegotiationEntity> GetNegotiation(Guid clientId, long productId)
    {
        var negotiationEntity = await _negotiationRepository.GetNegotiation(clientId, productId);
        if (negotiationEntity == null)
        {
            throw new NegotiationDoesNotExistException();
        }

        return negotiationEntity;
    }

    public async Task<PropositionEntity> PostProposition(long negotiationId, decimal price)
    {
        var negotiationEntity = await _negotiationRepository.GetNegotiation(negotiationId);
        if (negotiationEntity == null)
        {
            throw new NegotiationNotFoundException();
        }

        if (negotiationEntity.Finished)
        {
            throw new NegotiationHasEndedException();
        }

        if (negotiationEntity.Proposition.Count >= 3)
        {
            throw new PropositionsLimitReachedException();
        }

        var lastPropositionEntity = negotiationEntity.Proposition.MaxBy(x => x.ProposedAt);

        if (lastPropositionEntity != null)
        {
            var daysSinceLast = (DateTimeOffset.UtcNow - lastPropositionEntity.ProposedAt).TotalDays;
            if (daysSinceLast > 7)
            {
                throw new TimeForNewPropositionHasPassedException();
            }
        }

        var propositionEntity = new PropositionEntity
        {
            NegotiationId = negotiationId,
            ProposedPrice = price,
            ProposedAt = DateTime.UtcNow
        };

        await _propositionRepository.InsertPropositionAsync(propositionEntity);
        await _propositionRepository.SaveAsync();

        return propositionEntity;
    }
}
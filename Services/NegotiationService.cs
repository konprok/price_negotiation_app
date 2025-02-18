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
    private readonly IUserRepository _userRepository;

    public NegotiationService(INegotiationRepository negotiationRepository, IProductRepository productRepository,
        IPropositionRepository propositionRepository, IUserRepository userRepository)
    {
        _negotiationRepository = negotiationRepository;
        _productRepository = productRepository;
        _propositionRepository = propositionRepository;
        _userRepository = userRepository;
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

    public async Task<PropositionEntity> PostProposition(Guid clientId, long productId, decimal price)
    {
        if (price <= 0)
        {
            throw new InvalidInputException();
        }

        var negotiationEntity = await _negotiationRepository.GetNegotiation(clientId, productId);
        if (negotiationEntity == null)
        {
            var productEntity = await _productRepository.GetProduct(productId);
            var newNegotiationEntity = new NegotiationEntity
            {
                ProductId = productId,
                OwnerId = productEntity.OwnerId,
                ClientId = clientId,
                Finished = false,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _negotiationRepository.InsertNegotiationAsync(newNegotiationEntity);
            await _negotiationRepository.SaveAsync();

            var propositionEntity = new PropositionEntity
            {
                NegotiationId = newNegotiationEntity.Id,
                ProposedPrice = price,
                ProposedAt = DateTimeOffset.UtcNow
            };

            await _propositionRepository.InsertPropositionAsync(propositionEntity);
            await _propositionRepository.SaveAsync();

            return propositionEntity;
        }
        else
        {
            if (negotiationEntity.Finished)
            {
                throw new NegotiationHasEndedException();
            }

            if (negotiationEntity.Proposition.Count >= 3)
            {
                throw new PropositionsLimitReachedException();
            }

            var lastPropositionEntity = negotiationEntity.Proposition.MaxBy(x => x.ProposedAt);

            if (lastPropositionEntity.Decision == null)
            {
                throw new PropositionUnderConsiderationException();
            }

            var daysSinceLast = (DateTimeOffset.UtcNow - lastPropositionEntity.ProposedAt).TotalDays;
            if (daysSinceLast > 7)
            {
                throw new TimeForNewPropositionHasPassedException();
            }

            var propositionEntity = new PropositionEntity
            {
                NegotiationId = negotiationEntity.Id,
                ProposedPrice = price,
                ProposedAt = DateTimeOffset.UtcNow
            };

            negotiationEntity.ModyfiedAt = DateTimeOffset.UtcNow;

            await _propositionRepository.InsertPropositionAsync(propositionEntity);
            await _propositionRepository.SaveAsync();

            return propositionEntity;
        }
    }

    public async Task<NegotiationEntity> PatchProposition(Guid userId, long negotiationId, bool response)
    {
        var negotiationEntity = await _negotiationRepository.GetNegotiation(negotiationId);
        if (negotiationEntity == null)
        {
            throw new NegotiationNotFoundException();
        }

        if (negotiationEntity.OwnerId != userId)
        {
            throw new NegotiationNotFoundException();
        }

        var lastPropositionEntity = negotiationEntity.Proposition.MaxBy(x => x.ProposedAt);
        lastPropositionEntity.Decision = response;
        lastPropositionEntity.DecidedAt = DateTimeOffset.UtcNow;
        negotiationEntity.ModyfiedAt = DateTimeOffset.UtcNow;

        if (response)
        {
            negotiationEntity.Finished = true;
            negotiationEntity.FinalPrice = lastPropositionEntity.ProposedPrice;
        }

        if (negotiationEntity.Proposition.Count == 3)
        {
            negotiationEntity.Finished = true;
            negotiationEntity.FinalPrice = 0;
        }

        await _negotiationRepository.SaveAsync();

        return negotiationEntity;
    }

    public async Task<IEnumerable<NegotiationEntity?>> GetNegotiations(Guid userId)
    {
        if (await _userRepository.CheckUserById(userId))
        {
            return await _negotiationRepository.GetNegotiationsByOwnerId(userId);
        }

        return await _negotiationRepository.GetNegotiationsByClientId(userId);
    }
}
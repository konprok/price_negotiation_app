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
            throw new NotFoundException(ErrorMessages.NegotiationDoesNotExist);
        }

        return negotiationEntity;
    }
    
    public async Task<PropositionEntity> PostProposition(Guid clientId, long productId, decimal price)
    {
        ValidatePrice(price);

        var negotiationEntity = await _negotiationRepository.GetNegotiation(clientId, productId);

        if (negotiationEntity == null)
        {
            return await CreateNewNegotiationWithProposition(clientId, productId, price);
        }

        ValidateNegotiationRules(negotiationEntity);

        return await AddPropositionToExistingNegotiation(negotiationEntity, price);
    }
    
    public async Task<NegotiationEntity> PatchProposition(Guid userId, long negotiationId, bool response)
    {
        var negotiationEntity = await _negotiationRepository.GetNegotiation(negotiationId);
        if (negotiationEntity == null || negotiationEntity.OwnerId != userId)
        {
            throw new NotFoundException(ErrorMessages.NegotiationNotFound);
        }

        var lastPropositionEntity = negotiationEntity.Proposition.MaxBy(x => x.ProposedAt);
        lastPropositionEntity.IsAccepted = response;
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
        var userEntity = await _userRepository.GetUser(userId);
        return userEntity != null
            ? await _negotiationRepository.GetNegotiationsByOwnerId(userId)
            : await _negotiationRepository.GetNegotiationsByClientId(userId);
    }

    private void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new InvalidArgumentException(ErrorMessages.InvalidInput);
        }
    }

    private async Task<PropositionEntity> CreateNewNegotiationWithProposition(Guid clientId, long productId, decimal price)
    {
        var productEntity = await _productRepository.GetProduct(productId);
        if (productEntity == null)
        {
            throw new NotFoundException(ErrorMessages.ProductNotFoundException);
        }

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

        return await AddPropositionToExistingNegotiation(newNegotiationEntity, price);
    }

    private void ValidateNegotiationRules(NegotiationEntity negotiation)
    {
        if (negotiation.Finished)
        {
            throw new ConflictException(ErrorMessages.NegotiationHasEnded);
        }

        if (negotiation.Proposition.Count >= 3)
        {
            throw new ConflictException(ErrorMessages.PropositionsLimitReached);
        }

        var lastPropositionEntity = negotiation.Proposition.MaxBy(x => x.ProposedAt);

        if (lastPropositionEntity?.IsAccepted == null)
        {
            throw new ConflictException(ErrorMessages.PropositionUnderConsideration);
        }

        if (lastPropositionEntity != null &&
            (DateTimeOffset.UtcNow - lastPropositionEntity.ProposedAt).TotalDays > 7)
        {
            throw new ConflictException(ErrorMessages.TimeForNewPropositionHasPassed);
        }
    }

    private async Task<PropositionEntity> AddPropositionToExistingNegotiation(NegotiationEntity negotiation, decimal price)
    {
        var newPropositionEntity = new PropositionEntity
        {
            NegotiationId = negotiation.Id,
            ProposedPrice = price,
            ProposedAt = DateTimeOffset.UtcNow
        };

        negotiation.ModyfiedAt = DateTimeOffset.UtcNow;

        await _propositionRepository.InsertPropositionAsync(newPropositionEntity);
        await _propositionRepository.SaveAsync();

        return newPropositionEntity;
    }
}

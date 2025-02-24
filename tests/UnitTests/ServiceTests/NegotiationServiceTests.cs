namespace PriceNegotiationAppTests.UnitTests.ServiceTests;

public sealed class NegotiationServiceTests
{
    private INegotiationRepository _negotiationRepository = null!;
    private IProductRepository _productRepository = null!;
    private IPropositionRepository _propositionRepository = null!;
    private IUserRepository _userRepository = null!;
    private NegotiationService _negotiationService = null!;

    [SetUp]
    public void SetUp()
    {
        _negotiationRepository = Substitute.For<INegotiationRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _propositionRepository = Substitute.For<IPropositionRepository>();
        _userRepository = Substitute.For<IUserRepository>();

        _negotiationService = new NegotiationService(
            _negotiationRepository, _productRepository, _propositionRepository, _userRepository);
    }

    #region GetNegotiation Tests

    [Test]
    public async Task ShouldReturnNegotiationWhenNegotiationExists()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var negotiationEntity = new NegotiationEntity { Id = 1, ClientId = clientId, ProductId = productId };

        _negotiationRepository.GetNegotiation(clientId, productId).Returns(negotiationEntity);

        var result = await _negotiationService.GetNegotiation(clientId, productId);

        ClassicAssert.IsNotNull(result);
        Assert.That(result, Is.EqualTo(negotiationEntity));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenNegotiationDoesNotExist()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;

        _negotiationRepository.GetNegotiation(clientId, productId).Returns((NegotiationEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _negotiationService.GetNegotiation(clientId, productId));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.NegotiationDoesNotExist));
    }

    #endregion

    #region PostProposition Tests

    [Test]
    public async Task ShouldCreateNewNegotiationWhenNoExistingNegotiationFound()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var price = 500.0m;
        var productEntity = new ProductEntity { Id = productId, OwnerId = Guid.NewGuid() };

        _negotiationRepository.GetNegotiation(clientId, productId).Returns((NegotiationEntity)null!);
        _productRepository.GetProduct(productId).Returns(productEntity);

        var result = await _negotiationService.PostProposition(clientId, productId, price);

        ClassicAssert.IsNotNull(result);
        await _negotiationRepository.Received(1).InsertNegotiationAsync(Arg.Any<NegotiationEntity>());
        await _negotiationRepository.Received(1).SaveAsync();
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenPriceIsZeroOrNegative()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var invalidPrice = -100.0m;

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() =>
            _negotiationService.PostProposition(clientId, productId, invalidPrice));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.InvalidInput));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenProductDoesNotExist()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var price = 500.0m;

        _negotiationRepository.GetNegotiation(clientId, productId).Returns((NegotiationEntity)null!);
        _productRepository.GetProduct(productId).Returns((ProductEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() =>
            _negotiationService.PostProposition(clientId, productId, price));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.ProductNotFoundException));
    }

    [Test]
    public void ShouldThrowConflictExceptionWhenPropositionIsUnderConsideration()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var price = 500.0m;
        var negotiationEntity = new NegotiationEntity
        {
            Proposition = new List<PropositionEntity> { new() { IsAccepted = null } }
        };

        _negotiationRepository.GetNegotiation(clientId, productId).Returns(negotiationEntity);

        var ex = Assert.ThrowsAsync<ConflictException>(() =>
            _negotiationService.PostProposition(clientId, productId, price));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PropositionUnderConsideration));
    }

    [Test]
    public void ShouldThrowConflictExceptionWhenNegotiationIsFinished()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var price = 500.0m;
        var negotiationEntity = new NegotiationEntity { Finished = true };

        _negotiationRepository.GetNegotiation(clientId, productId).Returns(negotiationEntity);

        var ex = Assert.ThrowsAsync<ConflictException>(() =>
            _negotiationService.PostProposition(clientId, productId, price));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.NegotiationHasEnded));
    }

    [Test]
    public void ShouldThrowConflictExceptionWhenPropositionsLimitReached()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var price = 500.0m;
        var negotiationEntity = new NegotiationEntity
        {
            Proposition = new List<PropositionEntity> { new(), new(), new() }
        };

        _negotiationRepository.GetNegotiation(clientId, productId).Returns(negotiationEntity);

        var ex = Assert.ThrowsAsync<ConflictException>(() =>
            _negotiationService.PostProposition(clientId, productId, price));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.PropositionsLimitReached));
    }

    [Test]
    public void ShouldThrowConflictExceptionWhenTimeForNewPropositionHasPassed()
    {
        var clientId = Guid.NewGuid();
        var productId = 1;
        var price = 500.0m;
        var negotiationEntity = new NegotiationEntity
        {
            Proposition = new List<PropositionEntity>
            {
                new()
                {
                    ProposedAt = DateTimeOffset.UtcNow.AddDays(-8),
                    IsAccepted = false
                }
            },
            Finished = false
        };

        _negotiationRepository.GetNegotiation(clientId, productId).Returns(negotiationEntity);

        var ex = Assert.ThrowsAsync<ConflictException>(() =>
            _negotiationService.PostProposition(clientId, productId, price));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.TimeForNewPropositionHasPassed));
    }

    #endregion

    #region PatchProposition Tests

    [Test]
    public async Task ShouldUpdateNegotiationWhenValidPatchProposition()
    {
        var userId = Guid.NewGuid();
        var negotiationId = 1;
        var response = true;
        var negotiationEntity = new NegotiationEntity
        {
            Id = negotiationId,
            OwnerId = userId,
            Proposition = new List<PropositionEntity>
            {
                new PropositionEntity { ProposedAt = DateTimeOffset.UtcNow, ProposedPrice = 500m }
            }
        };

        _negotiationRepository.GetNegotiation(negotiationId).Returns(negotiationEntity);

        var result = await _negotiationService.PatchProposition(userId, negotiationId, response);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsTrue(result.Finished);
        await _negotiationRepository.Received(1).SaveAsync();
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenNegotiationDoesNotExistOnPatch()
    {
        var userId = Guid.NewGuid();
        var negotiationId = 1;
        var response = true;

        _negotiationRepository.GetNegotiation(negotiationId).Returns((NegotiationEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() =>
            _negotiationService.PatchProposition(userId, negotiationId, response));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.NegotiationNotFound));
    }

    #endregion

    #region GetNegotiations Tests

    [Test]
    public async Task ShouldReturnNegotiationsByOwnerWhenUserExists()
    {
        var userId = Guid.NewGuid();
        var negotiations = new List<NegotiationEntity>
        {
            new NegotiationEntity { Id = 1, OwnerId = userId },
            new NegotiationEntity { Id = 2, OwnerId = userId }
        };

        UserRegisterDto userRegisterDto = new UserRegisterDto("TestUser", "test@test.com", "password");

        _userRepository.GetUser(userId).Returns(new UserEntity(userRegisterDto));
        _negotiationRepository.GetNegotiationsByOwnerId(userId).Returns(negotiations);

        var result = await _negotiationService.GetNegotiations(userId);

        ClassicAssert.IsNotNull(result);
        Assert.That(((List<NegotiationEntity>)result).Count, Is.EqualTo(2));
    }

    [Test]
    public async Task ShouldReturnNegotiationsByClientWhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var negotiations = new List<NegotiationEntity>
        {
            new NegotiationEntity { Id = 1, ClientId = userId },
            new NegotiationEntity { Id = 2, ClientId = userId }
        };

        _userRepository.GetUser(userId).Returns((UserEntity)null!);
        _negotiationRepository.GetNegotiationsByClientId(userId).Returns(negotiations);

        var result = await _negotiationService.GetNegotiations(userId);

        ClassicAssert.IsNotNull(result);
        Assert.That(((List<NegotiationEntity>)result).Count, Is.EqualTo(2));
    }

    #endregion
}
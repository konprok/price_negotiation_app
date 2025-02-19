namespace PriceNegotiationAppTests.UnitTests.ControllerTests;

public class NegotiationControllerTest
{
    private INegotiationService _negotiationService = null!;
    private NegotiationController _negotiationController = null!;

    [SetUp]
    public void SetUp()
    {
        _negotiationService = Substitute.For<INegotiationService>();
        _negotiationController = new NegotiationController(_negotiationService);
    }

    [TearDown]
    public void TearDown()
    {
        (_negotiationController as IDisposable)?.Dispose();
    }

    public void Dispose()
    {
        TearDown();
    }

    #region PostProposition Tests

    [Test]
    public async Task ShouldReturnOkAfterPostProposition()
    {
        var postPropositionDto = new PostPropositionDto { ClientId = Guid.NewGuid(), ProductId = 1, Price = 500.0m };
        var propositionEntity = new PropositionEntity { Id = 1, ProposedPrice = postPropositionDto.Price };

        _negotiationService.PostProposition(postPropositionDto.ClientId, postPropositionDto.ProductId,
                postPropositionDto.Price)
            .Returns(propositionEntity);

        var result = await _negotiationController.PostProposition(postPropositionDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(propositionEntity));
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterPostPropositionWhenNegotiationNotFound()
    {
        var postPropositionDto = new PostPropositionDto { ClientId = Guid.NewGuid(), ProductId = 1, Price = 500.0m };

        _negotiationService.PostProposition(postPropositionDto.ClientId, postPropositionDto.ProductId,
                postPropositionDto.Price)
            .Returns(Task.FromException<PropositionEntity>(new NotFoundException("Negotiation not found")));

        var result = await _negotiationController.PostProposition(postPropositionDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnBadRequestAfterPostPropositionWhenInvalidArgumentExceptionThrown()
    {
        var postPropositionDto = new PostPropositionDto { ClientId = Guid.NewGuid(), ProductId = 1, Price = -50.0m };

        _negotiationService.PostProposition(postPropositionDto.ClientId, postPropositionDto.ProductId,
                postPropositionDto.Price)
            .Returns(Task.FromException<PropositionEntity>(new InvalidArgumentException("Invalid price")));

        var result = await _negotiationController.PostProposition(postPropositionDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnConflictAfterPostPropositionWhenConflictExceptionThrown()
    {
        var postPropositionDto = new PostPropositionDto { ClientId = Guid.NewGuid(), ProductId = 1, Price = 500.0m };

        _negotiationService.PostProposition(postPropositionDto.ClientId, postPropositionDto.ProductId,
                postPropositionDto.Price)
            .Returns(Task.FromException<PropositionEntity>(new ConflictException("Negotiation already closed")));

        var result = await _negotiationController.PostProposition(postPropositionDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ConflictObjectResult>(result.Result);
    }

    #endregion

    #region PatchProposition Tests

    [Test]
    public async Task ShouldReturnOkAfterPatchProposition()
    {
        var patchPropositionDto = new PatchPropositionDto
            { UserId = Guid.NewGuid(), NegotiationId = 1, Response = true };
        var negotiationEntity = new NegotiationEntity { Id = 1, Finished = true, FinalPrice = 500.0m };

        _negotiationService.PatchProposition(patchPropositionDto.UserId, patchPropositionDto.NegotiationId,
                patchPropositionDto.Response)
            .Returns(negotiationEntity);

        var result = await _negotiationController.PatchProposition(patchPropositionDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(negotiationEntity));
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterPatchPropositionWhenNegotiationNotFound()
    {
        var patchPropositionDto = new PatchPropositionDto
            { UserId = Guid.NewGuid(), NegotiationId = 1, Response = true };

        _negotiationService.PatchProposition(patchPropositionDto.UserId, patchPropositionDto.NegotiationId,
                patchPropositionDto.Response)
            .Returns(Task.FromException<NegotiationEntity>(new NotFoundException("Negotiation not found")));

        var result = await _negotiationController.PatchProposition(patchPropositionDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region GetNegotiations Tests

    [Test]
    public async Task ShouldReturnOkAfterGetNegotiations()
    {
        var userId = Guid.NewGuid();
        var negotiations = new List<NegotiationEntity>
        {
            new NegotiationEntity { Id = 1, Finished = false },
            new NegotiationEntity { Id = 2, Finished = true }
        };

        _negotiationService.GetNegotiations(userId).Returns(negotiations);

        var result = await _negotiationController.GetNegotiations(userId);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(negotiations));
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterGetNegotiationsWhenUnhandledExceptionThrown()
    {
        var userId = Guid.NewGuid();

        _negotiationService.GetNegotiations(userId)
            .Returns(Task.FromException<IEnumerable<NegotiationEntity>>(new Exception("Unexpected error")));

        var result = await _negotiationController.GetNegotiations(userId);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);
    }

    #endregion
}
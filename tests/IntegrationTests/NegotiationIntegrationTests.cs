namespace PriceNegotiationAppTests.IntegrationTests;

public sealed class NegotiationControllerIntegrationTests : WebApplicationFactory<Program>
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _httpClient = null!;
    private AppDbContext _dbContext = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("TestDatabase"); });
                });
            });
        _dbContext = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        _httpClient = _factory.CreateClient();
    }

    [SetUp]
    public void SetUp()
    {
        _dbContext.Products.Add(new ProductEntity()
        {
            Id = 1,
            Name = "Test Product2",
            Description = "Test Description",
            BasePrice = 200.00m,
            OwnerId = new Guid("00000000-0000-0000-0000-000000000001"),
        });
        _dbContext.Propositions.Add(new PropositionEntity()
        {
            Id = 1,
            NegotiationId = 1,
            ProposedPrice = 180.00m,
            ProposedAt = DateTimeOffset.UtcNow - TimeSpan.FromDays(1),
            Negotiation = new NegotiationEntity()
            {
                Id = 1,
                ProductId = 2,
                OwnerId = new Guid("00000000-0000-0000-0000-000000000001"),
                ClientId = new Guid("00000000-0000-0000-0000-000000000002"),
                Finished = false
            }
        });
        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.ChangeTracker.Clear();
        _dbContext.Products.RemoveRange(_dbContext.Products);
        _dbContext.Negotiations.RemoveRange(_dbContext.Negotiations);
        _dbContext.Propositions.RemoveRange(_dbContext.Propositions);
        _dbContext.SaveChanges();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _dbContext.Dispose();
        _httpClient.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task ShouldCreatePropositionSuccessfully()
    {
        var proposition = new PostPropositionDto
        {
            ClientId = Guid.NewGuid(),
            ProductId = 1,
            Price = 150.00m
        };

        var content = new StringContent(JsonSerializer.Serialize(proposition), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/negotiations/proposition", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnNotFoundWhenCreatingPropositionForNonexistentProduct()
    {
        var proposition = new PostPropositionDto
        {
            ClientId = Guid.NewGuid(),
            ProductId = 9999,
            Price = 150.00m
        };

        var content = new StringContent(JsonSerializer.Serialize(proposition), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/negotiations/proposition", content);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldPatchPropositionSuccessfully()
    {
        var patchProposition = new PatchPropositionDto
        {
            UserId = new Guid("00000000-0000-0000-0000-000000000001"),
            NegotiationId = 1,
            Response = true
        };

        var content = new StringContent(JsonSerializer.Serialize(patchProposition), Encoding.UTF8, "application/json");
        var result = await _httpClient.PatchAsync("/negotiations/proposition", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnNotFoundForNonexistentNegotiationOnPatch()
    {
        var patchProposition = new PatchPropositionDto
        {
            UserId = Guid.NewGuid(),
            NegotiationId = 9999,
            Response = true
        };

        var content = new StringContent(JsonSerializer.Serialize(patchProposition), Encoding.UTF8, "application/json");
        var result = await _httpClient.PatchAsync("/negotiations/proposition", content);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldReturnNegotiationsForUser()
    {
        var userId = new Guid("00000000-0000-0000-0000-000000000002");
        var result = await _httpClient.GetAsync($"/negotiations/user/{userId}");

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
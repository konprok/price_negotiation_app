namespace PriceNegotiationAppTests.IntegrationTests;

public sealed class ProductControllerIntegrationTests : WebApplicationFactory<Program>
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _httpClient = null!;
    private AppDbContext _dbContext = null!;
    private IPasswordHasher _passwordHasher = null!;

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
        _passwordHasher = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<IPasswordHasher>();
        _httpClient = _factory.CreateClient();
    }

    [SetUp]
    public void SetUp()
    {
        _dbContext.Users.Add(new UserEntity
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            UserName = "testUser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.Hash("password")
        });
        _dbContext.Products.Add(new ProductEntity()
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            BasePrice = 100.00m,
            OwnerId = new Guid("00000000-0000-0000-0000-000000000001"),
        });
        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.ChangeTracker.Clear();
        _dbContext.Products.RemoveRange(_dbContext.Products);
        _dbContext.Users.RemoveRange(_dbContext.Users);
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
    public async Task ShouldCreateProductSuccessfully()
    {
        var userId = new Guid("00000000-0000-0000-0000-000000000001");
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            BasePrice = 100.00m
        };

        var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync($"/products/user/{userId}", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnNotFoundWhenCreatingProductForNonexistentUser()
    {
        var userId = Guid.NewGuid();
        var product = new Product
        {
            Name = "Invalid Product",
            Description = "Invalid Description",
            BasePrice = 50.00m,
        };

        var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync($"/products/user/{userId}", content);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldReturnProductById()
    {
        var productId = 1;
        var result = await _httpClient.GetAsync($"/products/{productId}");

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnNotFoundForNonexistentProduct()
    {
        var productId = 9999;
        var result = await _httpClient.GetAsync($"/products/{productId}");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldReturnProductsByUser()
    {
        var userId = new Guid("00000000-0000-0000-0000-000000000001");
        var result = await _httpClient.GetAsync($"/products/user/{userId}");

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnAllProducts()
    {
        var result = await _httpClient.GetAsync("/products/all");

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
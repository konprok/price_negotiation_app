namespace PriceNegotiationAppTests.IntegrationTests;

public sealed class UserControllerIntegrationTests : WebApplicationFactory<Program>
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
            Id = Guid.NewGuid(),
            UserName = "testUser",
            Email = "test@example.com",
            PasswordHash = _passwordHasher.Hash("password")
        });
        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
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
    public async Task ShouldRegisterUserSuccessfully()
    {
        var newUser = new UserRegisterDto
        {
            UserName = "newUser",
            Email = "newuser@example.com",
            Password = "securepassword"
        };

        var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/register", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnBadRequestOnInvalidRegistration()
    {
        var invalidUser = new UserRegisterDto
        {
            UserName = "",
            Email = "invalidEmail",
            Password = "short"
        };

        var content = new StringContent(JsonSerializer.Serialize(invalidUser), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/register", content);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ShouldLoginSuccessfully()
    {
        var loginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "password"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/login", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnNotFoundForNonexistentUserLogin()
    {
        var loginDto = new UserLoginDto
        {
            Email = "nonexistent@example.com",
            Password = "somepassword"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/login", content);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldReturnBadRequestForInvalidPassword()
    {
        var loginDto = new UserLoginDto
        {
            Password = "wrongpassword",
            Email = "test@example.com"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/login", content);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
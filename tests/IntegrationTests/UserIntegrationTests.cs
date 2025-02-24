namespace PriceNegotiationAppTests.IntegrationTests;

public sealed class UserControllerIntegrationTests : WebApplicationFactory<Program>
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _httpClient = null!;
    private AppDbContext _dbContext = null!;
    private UserRegisterDto _userRegisterDto = null!;
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
        _httpClient = _factory.CreateClient();
    }

    [SetUp]
    public void SetUp()
    {
        _passwordHasher = new PasswordHascher();
        _userRegisterDto = new UserRegisterDto("user@example.com", "user", _passwordHasher.Hash("password"));
        _dbContext.Users.Add(new UserEntity(_userRegisterDto)
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001")
        });
        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.ChangeTracker.Clear();
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
        var newUser = new UserRegisterDto("newuser@example.com", "newUser", "securepassword");

        var content = new StringContent(JsonSerializer.Serialize(newUser), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/register", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnBadRequestOnInvalidRegistration()
    {
        var invalidUser = new UserRegisterDto("invalidEmail", "", "short");

        var content = new StringContent(JsonSerializer.Serialize(invalidUser), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/register", content);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ShouldLoginSuccessfully()
    {
        var loginDto = new UserLoginDto("user@example.com", "password");

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/login", content);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ShouldReturnNotFoundForNonexistentUserLogin()
    {
        var loginDto = new UserLoginDto("nonexistent@example.com", "password");

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/login", content);

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ShouldReturnBadRequestForInvalidPassword()
    {
        var loginDto = new UserLoginDto("user@example.com", "wrongpassword");

        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/users/login", content);

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
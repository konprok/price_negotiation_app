namespace PriceNegotiationAppTests.UnitTests.ServiceTests;

public class UserServiceTest
{
    private IUserRepository _userRepository = null!;
    private IPasswordHasher _passwordHasher = null!;
    private IValidator<UserRegisterDto> _userRegisterValidator = null!;
    private UserService _userService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _userRegisterValidator = Substitute.For<IValidator<UserRegisterDto>>();

        _userService = new UserService(_userRepository, _passwordHasher, _userRegisterValidator);
    }

    #region PostUser Tests

    [Test]
    public async Task ShouldCreateUserWhenValidUserRegisterDtoProvided()
    {
        var userRegisterDto = new UserRegisterDto
            { UserName = "TestUser", Email = "test@test.com", Password = "Pass123" };
        var userEntity = new UserEntity
            { UserName = "TestUser", Email = "test@test.com", PasswordHash = "hashedpassword" };

        _userRegisterValidator.ValidateAsync(userRegisterDto).Returns(new ValidationResult());
        _passwordHasher.Hash(userRegisterDto.Password).Returns("hashedpassword");

        var result = await _userService.PostUser(userRegisterDto);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo(userRegisterDto.UserName));
        Assert.That(result.Email, Is.EqualTo(userRegisterDto.Email));

        await _userRepository.Received(1).InsertUserAsync(Arg.Is<UserEntity>(u =>
            u.UserName == userRegisterDto.UserName &&
            u.Email == userRegisterDto.Email &&
            u.PasswordHash == "hashedpassword"));

        await _userRepository.Received(1).SaveAsync();
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenValidationFails()
    {
        var userRegisterDto = new UserRegisterDto();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Invalid email"),
            new ValidationFailure("UserName", "Username required")
        };
        _userRegisterValidator.ValidateAsync(userRegisterDto).Returns(new ValidationResult(validationFailures));

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.PostUser(userRegisterDto));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Does.Contain("Invalid email"));
        Assert.That(ex.Message, Does.Contain("Username required"));

        _userRepository.DidNotReceive().InsertUserAsync(Arg.Any<UserEntity>());
        _userRepository.DidNotReceive().SaveAsync();
    }

    #endregion

    #region GetUser (Email & Password) Tests

    [Test]
    public async Task ShouldReturnUserWhenValidCredentialsProvided()
    {
        string email = "test@test.com";
        string password = "Pass123";
        var userEntity = new UserEntity { Email = email, PasswordHash = "hashedpassword", UserName = "TestUser" };

        _userRepository.GetUser(email).Returns(userEntity);
        _passwordHasher.Verify(password, "hashedpassword").Returns(true);

        var result = await _userService.GetUser(email, password);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Email, Is.EqualTo(email));
        Assert.That(result.Name, Is.EqualTo("TestUser"));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenEmailIsNull()
    {
        string email = null!;
        string password = "Pass123";

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.GetUser(email, password));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.InvalidUser));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserDoesNotExist()
    {
        string email = "test@test.com";
        string password = "Pass123";

        _userRepository.GetUser(email).Returns((UserEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUser(email, password));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenPasswordIsIncorrect()
    {
        string email = "test@test.com";
        string password = "WrongPass";
        var userEntity = new UserEntity { Email = email, PasswordHash = "hashedpassword", UserName = "TestUser" };

        _userRepository.GetUser(email).Returns(userEntity);
        _passwordHasher.Verify(password, "hashedpassword").Returns(false);

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.GetUser(email, password));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.InvalidPassword));
    }

    #endregion

    #region GetUser (By UserId) Tests

    [Test]
    public async Task ShouldReturnUserWhenValidUserIdProvided()
    {
        var userId = Guid.NewGuid();
        var userEntity = new UserEntity
            { Id = userId, UserName = "TestUser", Email = "test@test.com", PasswordHash = "hashedpassword" };

        _userRepository.GetUser(userId).Returns(userEntity);

        var result = await _userService.GetUser(userId);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(userId));
        Assert.That(result.Name, Is.EqualTo("TestUser"));
        Assert.That(result.Email, Is.EqualTo("test@test.com"));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserIdNotFound()
    {
        var userId = Guid.NewGuid();

        _userRepository.GetUser(userId).Returns((UserEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUser(userId));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    #endregion
}
namespace PriceNegotiationAppTests.UnitTests.ServiceTests;

public sealed class UserServiceTests
{
    private IUserRepository _userRepository = null!;
    private IPasswordHasher _passwordHasher = null!;
    private IValidator<UserRegisterDto> _userRegisterValidator = null!;
    private UserService _userService = null!;
    private UserEntity _testUser = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _userRegisterValidator = Substitute.For<IValidator<UserRegisterDto>>();

        _userService = new UserService(_userRepository, _passwordHasher, _userRegisterValidator);

        _testUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "TestUser",
            Email = "test@test.com",
            PasswordHash = "hashedpassword"
        };
    }

    #region PostUser Tests

    [Test]
    public async Task ShouldCreateUserWhenValidUserRegisterDtoProvided()
    {
        var userRegisterDto = new UserRegisterDto(_testUser.UserName, _testUser.Email, "Pass123");

        _userRegisterValidator.ValidateAsync(userRegisterDto).Returns(new ValidationResult());
        _passwordHasher.Hash(userRegisterDto.Password).Returns(_testUser.PasswordHash);

        var result = await _userService.PostUser(userRegisterDto);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Name, Is.EqualTo(_testUser.UserName));
        Assert.That(result.Email, Is.EqualTo(_testUser.Email));

        await _userRepository.Received(1).InsertUserAsync(Arg.Is<UserEntity>(u =>
            u.UserName == _testUser.UserName &&
            u.Email == _testUser.Email &&
            u.PasswordHash == _testUser.PasswordHash));

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
        _userRepository.GetUser(_testUser.Email).Returns(_testUser);
        _passwordHasher.Verify("Pass123", _testUser.PasswordHash).Returns(true);

        var result = await _userService.GetUser(_testUser.Email, "Pass123");

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Email, Is.EqualTo(_testUser.Email));
        Assert.That(result.Name, Is.EqualTo(_testUser.UserName));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenEmailIsNull()
    {
        string? email = null;
        string password = "Pass123";

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.GetUser(email, password));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.InvalidUser));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserDoesNotExist()
    {
        _userRepository.GetUser(_testUser.Email).Returns((UserEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUser(_testUser.Email, "Pass123"));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenPasswordIsIncorrect()
    {
        _userRepository.GetUser(_testUser.Email).Returns(_testUser);
        _passwordHasher.Verify("WrongPass", _testUser.PasswordHash).Returns(false);

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.GetUser(_testUser.Email, "WrongPass"));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.InvalidPassword));
    }

    #endregion

    #region GetUser (By UserId) Tests

    [Test]
    public async Task ShouldReturnUserWhenValidUserIdProvided()
    {
        _userRepository.GetUser(_testUser.Id).Returns(_testUser);

        var result = await _userService.GetUser(_testUser.Id);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(_testUser.Id));
        Assert.That(result.Name, Is.EqualTo(_testUser.UserName));
        Assert.That(result.Email, Is.EqualTo(_testUser.Email));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserIdNotFound()
    {
        var nonExistentUserId = Guid.NewGuid();
        _userRepository.GetUser(nonExistentUserId).Returns((UserEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUser(nonExistentUserId));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    #endregion
}
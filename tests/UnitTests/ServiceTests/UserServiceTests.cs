namespace PriceNegotiationAppTests.UnitTests.ServiceTests;

public sealed class UserServiceTests
{
    private IUserRepository _userRepository = null!;
    private IPasswordHasher _passwordHasher = null!;
    private IValidator<UserRegisterDto> _userRegisterValidator = null!;
    private UserService _userService = null!;
    private UserRegisterDto _userRegisterDto = null!;
    private UserLoginDto _userLoginDto = null!;
    private UserEntity _testUser = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _userRegisterValidator = Substitute.For<IValidator<UserRegisterDto>>();

        _userService = new UserService(_userRepository, _passwordHasher, _userRegisterValidator);
        _userRegisterDto = new UserRegisterDto("user@example.com", "user", "password");
        _testUser = new UserEntity(_userRegisterDto);
        _userLoginDto = new UserLoginDto(_testUser.Email, _testUser.PasswordHash);
    }

    #region PostUser Tests

    [Test]
    public async Task ShouldCreateUserWhenValidUserRegisterDtoProvided()
    {
        var userRegisterDto = new UserRegisterDto(_testUser.Email, _testUser.UserName, "password");

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
        var userRegisterDto = new UserRegisterDto("user@exampl.com", "user", "password");
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
        _passwordHasher.Verify("password", _testUser.PasswordHash).Returns(true);
        var result = await _userService.GetUser(_userLoginDto);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Email, Is.EqualTo(_testUser.Email));
        Assert.That(result.Name, Is.EqualTo(_testUser.UserName));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenEmailIsNull()
    {
        UserLoginDto userLoginDto = new UserLoginDto(null, "password");

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.GetUser(userLoginDto));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.InvalidInput));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserDoesNotExist()
    {
        _userRepository.GetUser(_testUser.Email).Returns((UserEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUser(_userLoginDto));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenPasswordIsIncorrect()
    {
        _userRepository.GetUser(_testUser.Email).Returns(_testUser);
        _passwordHasher.Verify("WrongPass", _testUser.PasswordHash).Returns(false);
        UserLoginDto userLoginDto = new UserLoginDto(_testUser.Email, "wrongPassword");

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _userService.GetUser(userLoginDto));

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
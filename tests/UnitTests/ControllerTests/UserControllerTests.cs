namespace PriceNegotiationAppTests.UnitTests.ControllerTests;

public sealed class UserControllerTests
{
    private IUserService _userService = null!;
    private UserController _userController = null!;
    private UserLoginDto _userLoginDto = null!;

    private UserRegisterDto _userRegisterDto = null!;


    [SetUp]
    public void SetUp()
    {
        _userService = Substitute.For<IUserService>();
        _userController = new UserController(_userService);
        _userLoginDto = new UserLoginDto("user@example.com", "password");
        _userRegisterDto = new UserRegisterDto("user@example.com", "user", "password");
    }

    #region PostUser Tests

    [Test]
    public async Task ShouldReturnOkAfterPostUser()
    {
        var userResponse = new UserResponse(new UserEntity(_userRegisterDto))
            { Name = "TestUser", Email = "test@test.com" };

        _userService.PostUser(Arg.Any<UserRegisterDto>()).Returns(userResponse);

        var result = await _userController.PostUser(_userRegisterDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(userResponse));
    }

    [Test]
    public async Task ShouldReturnBadRequestAfterPostUserWhenInvalidArgumentExceptionThrown()
    {
        _userService.PostUser(Arg.Any<UserRegisterDto>())
            .Returns(Task.FromException<UserResponse>(new InvalidArgumentException("Invalid data")));

        var result = await _userController.PostUser(_userRegisterDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        var badRequestResult = result.Result as BadRequestObjectResult;
        ClassicAssert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid data"));
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterPostUserWhenUnhandledExceptionThrown()
    {
        var userRegisterDto = new UserRegisterDto("user@example.com", "user", "password");
        _userService.PostUser(Arg.Any<UserRegisterDto>())
            .Returns(Task.FromException<UserResponse>(new Exception("Unexpected error")));

        var result = await _userController.PostUser(userRegisterDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);

        var objectResult = result.Result as ObjectResult;
        ClassicAssert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.EqualTo("Unexpected error"));
    }

    #endregion

    #region PostUserLogin Tests

    [Test]
    public async Task ShouldReturnOkAfterPostUserLogin()
    {
        UserRegisterDto userRegisterDto = new("user@example.com", "user", "password");
        var userResponse = new UserResponse(new UserEntity(userRegisterDto));

        _userService.GetUser(Arg.Any<UserLoginDto>()).Returns(userResponse);

        var result = await _userController.PostUserLogin(_userLoginDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(userResponse));
    }

    [Test]
    public async Task ShouldReturnBadRequestAfterPostUserLoginWhenInvalidArgumentExceptionThrown()
    {
        _userService.GetUser(Arg.Any<UserLoginDto>())
            .Returns(Task.FromException<UserResponse>(new InvalidArgumentException("Invalid credentials")));

        var result = await _userController.PostUserLogin(_userLoginDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        var badRequestResult = result.Result as BadRequestObjectResult;
        ClassicAssert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid credentials"));
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterPostUserLoginWhenNotFoundExceptionThrown()
    {
        _userService.GetUser(Arg.Any<UserLoginDto>())
            .Returns(Task.FromException<UserResponse>(new NotFoundException("User not found")));

        var result = await _userController.PostUserLogin(_userLoginDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);

        var notFoundResult = result.Result as NotFoundObjectResult;
        ClassicAssert.IsNotNull(notFoundResult);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterPostUserLogin_WhenUnhandledExceptionThrown()
    {
        _userService.GetUser(Arg.Any<UserLoginDto>())
            .Returns(Task.FromException<UserResponse>(new Exception("Unexpected error")));

        var result = await _userController.PostUserLogin(_userLoginDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);

        var objectResult = result.Result as ObjectResult;
        ClassicAssert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.EqualTo("Unexpected error"));
    }

    #endregion
}
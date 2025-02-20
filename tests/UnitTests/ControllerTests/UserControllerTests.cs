namespace PriceNegotiationAppTests.UnitTests.ControllerTests;

public sealed class UserControllerTests
{
    private IUserService _userService = null!;
    private UserController _userController = null!;

    [SetUp]
    public void SetUp()
    {
        _userService = Substitute.For<IUserService>();
        _userController = new UserController(_userService);
    }

    #region PostUser Tests

    [Test]
    public async Task ShouldReturnOkAfterPostUser()
    {
        var userRegisterDto = new UserRegisterDto
            { UserName = "TestUser", Email = "test@test.com", Password = "Pass123" };
        var userResponse = new UserResponse { Name = "TestUser", Email = "test@test.com" };

        _userService.PostUser(Arg.Any<UserRegisterDto>()).Returns(userResponse);

        var result = await _userController.PostUser(userRegisterDto);

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
        var userRegisterDto = new UserRegisterDto();
        _userService.PostUser(Arg.Any<UserRegisterDto>())
            .Returns(Task.FromException<UserResponse>(new InvalidArgumentException("Invalid data")));

        var result = await _userController.PostUser(userRegisterDto);

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
        var userRegisterDto = new UserRegisterDto();
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
        var userLoginDto = new UserLoginDto { Email = "test@test.com", Password = "Pass123" };
        var userResponse = new UserResponse { Name = "TestUser", Email = "test@test.com" };

        _userService.GetUser(Arg.Any<string>(), Arg.Any<string>()).Returns(userResponse);

        var result = await _userController.PostUserLogin(userLoginDto);

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
        var userLoginDto = new UserLoginDto();
        _userService.GetUser(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException<UserResponse>(new InvalidArgumentException("Invalid credentials")));

        var result = await _userController.PostUserLogin(userLoginDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        var badRequestResult = result.Result as BadRequestObjectResult;
        ClassicAssert.IsNotNull(badRequestResult);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.EqualTo("Invalid credentials"));
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterPostUserLogin_WhenNotFoundExceptionThrown()
    {
        var userLoginDto = new UserLoginDto();
        _userService.GetUser(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException<UserResponse>(new NotFoundException("User not found")));

        var result = await _userController.PostUserLogin(userLoginDto);

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
        var userLoginDto = new UserLoginDto();
        _userService.GetUser(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromException<UserResponse>(new Exception("Unexpected error")));

        var result = await _userController.PostUserLogin(userLoginDto);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);

        var objectResult = result.Result as ObjectResult;
        ClassicAssert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.EqualTo("Unexpected error"));
    }

    #endregion
}
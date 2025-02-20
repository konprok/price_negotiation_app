namespace PriceNegotiationAppTests.UnitTests.ServiceTests;

public sealed class ProductServiceTests
{
    private IProductRepository _productRepository = null!;
    private IUserRepository _userRepository = null!;
    private IValidator<Product> _productModelValidator = null!;
    private ProductService _productService = null!;

    [SetUp]
    public void SetUp()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _productModelValidator = Substitute.For<IValidator<Product>>();

        _productService = new ProductService(_productRepository, _userRepository, _productModelValidator);
    }

    #region PostProduct Tests

    [Test]
    public async Task ShouldCreateProduct()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };
        var userEntity = new UserEntity
            { Id = userId, UserName = "TestUser", Email = "test@test.com", PasswordHash = "hashedpassword" };

        _userRepository.GetUser(userId).Returns(userEntity);
        _productModelValidator.ValidateAsync(product).Returns(new ValidationResult());
        
        var result = await _productService.PostProduct(userId, product);
        
        ClassicAssert.IsNotNull(result);
        Assert.That(result.OwnerId, Is.EqualTo(userId));
        Assert.That(result.Name, Is.EqualTo(product.Name));

        await _productRepository.Received(1).InsertProductAsync(Arg.Any<ProductEntity>());
        await _productRepository.Received(1).SaveAsync();
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };

        _userRepository.GetUser(userId).Returns((UserEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _productService.PostProduct(userId, product));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    [Test]
    public void ShouldThrowInvalidArgumentExceptionWhenProductValidationFails()
    {
        var userId = Guid.NewGuid();
        var product = new Product();
        var userEntity = new UserEntity
            { Id = userId, UserName = "TestUser", Email = "test@test.com", PasswordHash = "hashedpassword" };

        _userRepository.GetUser(userId).Returns(userEntity);
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Product name is required")
        };
        _productModelValidator.ValidateAsync(product).Returns(new ValidationResult(validationFailures));

        var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => _productService.PostProduct(userId, product));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Does.Contain("Product name is required"));
    }

    #endregion

    #region GetProduct Tests

    [Test]
    public async Task ShouldReturnProductWhenValidProductIdProvided()
    {
        long productId = 1;
        var productEntity = new ProductEntity
            { Id = productId, Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };

        _productRepository.GetProduct(productId).Returns(productEntity);

        var result = await _productService.GetProduct(productId);

        ClassicAssert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(productId));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenProductNotFound()
    {
        long productId = 1;

        _productRepository.GetProduct(productId).Returns((ProductEntity)null!);

        var ex = Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProduct(productId));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.ProductNotFoundException));
    }

    #endregion

    #region GetProducts Tests

    [Test]
    public async Task ShouldReturnAllProductsWhenProductsExist()
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Laptop", BasePrice = 2500.99m },
            new ProductEntity { Id = 2, Name = "Phone", BasePrice = 999.99m }
        };

        _productRepository.GetProducts().Returns(products);

        var result = await _productService.GetProducts();

        ClassicAssert.IsNotNull(result);
        Assert.That(((List<ProductEntity>)result).Count, Is.EqualTo(2));
    }

    #endregion

    #region GetProductsByOwnerId Tests

    [Test]
    public async Task ShouldReturnProductsByUserIdWhenUserExists()
    {
        var userId = Guid.NewGuid();
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Laptop", BasePrice = 2500.99m, OwnerId = userId },
            new ProductEntity { Id = 2, Name = "Phone", BasePrice = 999.99m, OwnerId = userId }
        };
        var userEntity = new UserEntity
            { Id = userId, UserName = "TestUser", Email = "test@test.com", PasswordHash = "hashedpassword" };

        _userRepository.GetUser(userId).Returns(userEntity);
        _productRepository.GetProducts(userId).Returns(products);
        
        var result = await _productService.GetProductsByOwnerId(userId);
        
        ClassicAssert.IsNotNull(result);
        Assert.That(((List<ProductEntity>)result).Count, Is.EqualTo(2));
    }

    [Test]
    public void ShouldThrowNotFoundExceptionWhenUserNotFoundForGetProductsByOwnerId()
    {
        var userId = Guid.NewGuid();

        _userRepository.GetUser(userId).Returns((UserEntity)null!);
        
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProductsByOwnerId(userId));

        ClassicAssert.IsNotNull(ex);
        Assert.That(ex.Message, Is.EqualTo(ErrorMessages.UserNotFound));
    }

    #endregion
}
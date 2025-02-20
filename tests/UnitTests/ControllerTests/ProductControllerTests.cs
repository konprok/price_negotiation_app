namespace PriceNegotiationAppTests.UnitTests.ControllerTests;

public sealed class ProductControllerTests
{
    private IProductService _productService = null!;
    private ProductController _productController = null!;
    private Guid _testUserId;
    private Product _testProduct = null!;
    private ProductEntity _testProductEntity = null!;
    private List<ProductEntity> _testProductList = null!;

    [SetUp]
    public void SetUp()
    {
        _productService = Substitute.For<IProductService>();
        _productController = new ProductController(_productService);

        _testUserId = Guid.NewGuid();
        _testProduct = new Product { Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };
        _testProductEntity = new ProductEntity(_testProduct) { Id = 1, OwnerId = _testUserId };
        _testProductList = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Laptop", BasePrice = 2500.99m, OwnerId = _testUserId },
            new ProductEntity { Id = 2, Name = "Phone", BasePrice = 999.99m, OwnerId = _testUserId }
        };
    }

    #region PostProduct Tests

    [Test]
    public async Task ShouldReturnOkAfterPostProduct()
    {
        _productService.PostProduct(_testUserId, _testProduct).Returns(_testProductEntity);

        var result = await _productController.PostProduct(_testUserId, _testProduct);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        Assert.That(okResult?.Value, Is.EqualTo(_testProductEntity));
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterPostProductWhenUserDoesNotExist()
    {
        _productService.PostProduct(_testUserId, _testProduct)
            .Returns(Task.FromException<ProductEntity>(new NotFoundException("User not found")));

        var result = await _productController.PostProduct(_testUserId, _testProduct);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnBadRequestAfterPostProductWhenInvalidArgumentExceptionThrown()
    {
        _productService.PostProduct(_testUserId, _testProduct)
            .Returns(Task.FromException<ProductEntity>(new InvalidArgumentException("Invalid product")));

        var result = await _productController.PostProduct(_testUserId, _testProduct);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterPostProductWhenUnhandledExceptionThrown()
    {
        _productService.PostProduct(_testUserId, _testProduct)
            .Returns(Task.FromException<ProductEntity>(new Exception("Unexpected error")));

        var result = await _productController.PostProduct(_testUserId, _testProduct);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);
    }

    #endregion

    #region GetProduct Tests

    [Test]
    public async Task ShouldReturnOkAfterGetProduct()
    {
        _productService.GetProduct(_testProductEntity.Id).Returns(_testProductEntity);

        var result = await _productController.GetProduct(_testProductEntity.Id);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterGetProductWhenProductNotFound()
    {
        _productService.GetProduct(_testProductEntity.Id)
            .Returns(Task.FromException<ProductEntity>(new NotFoundException("Product not found")));

        var result = await _productController.GetProduct(_testProductEntity.Id);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterGetProductWhenUnhandledExceptionThrown()
    {
        _productService.GetProduct(_testProductEntity.Id)
            .Returns(Task.FromException<ProductEntity>(new Exception("Unexpected error")));

        var result = await _productController.GetProduct(_testProductEntity.Id);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);
    }

    #endregion

    #region GetProductsByUserId Tests

    [Test]
    public async Task ShouldReturnOkAfterGetProductsByUserId()
    {
        _productService.GetProductsByOwnerId(_testUserId).Returns(_testProductList);

        var result = await _productController.GetProducts(_testUserId);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterGetProductsByUserIdWhenUserNotFound()
    {
        _productService.GetProductsByOwnerId(_testUserId)
            .Returns(Task.FromException<IEnumerable<ProductEntity>>(new NotFoundException("User not found")));

        var result = await _productController.GetProducts(_testUserId);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region GetProducts Tests

    [Test]
    public async Task ShouldReturnOkAfterGetProducts()
    {
        _productService.GetProducts().Returns(_testProductList);

        var result = await _productController.GetProducts();

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterGetProductsWhenUnhandledExceptionThrown()
    {
        _productService.GetProducts()
            .Returns(Task.FromException<IEnumerable<ProductEntity>>(new Exception("Unexpected error")));

        var result = await _productController.GetProducts();

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);
    }

    #endregion
}
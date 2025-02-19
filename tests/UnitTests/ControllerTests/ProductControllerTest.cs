namespace PriceNegotiationAppTests.UnitTests.ControllerTests;

public class ProductControllerTest
{
    private IProductService _productService = null!;
    private ProductController _productController = null!;

    [SetUp]
    public void SetUp()
    {
        _productService = Substitute.For<IProductService>();
        _productController = new ProductController(_productService);
    }

    #region PostProduct Tests

    [Test]
    public async Task ShouldReturnOkAfterPostProduct()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };
        var productEntity = new ProductEntity(product) { Id = 1, OwnerId = userId };

        _productService.PostProduct(userId, product).Returns(productEntity);
        
        var result = await _productController.PostProduct(userId, product);
        
        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        ClassicAssert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(productEntity));
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterPostProductWhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };

        _productService.PostProduct(userId, product)
            .Returns(Task.FromException<ProductEntity>(new NotFoundException("User not found")));
        
        var result = await _productController.PostProduct(userId, product);
        
        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnBadRequestAfterPostProductWhenInvalidArgumentExceptionThrown()
    {
        var userId = Guid.NewGuid();
        var product = new Product();

        _productService.PostProduct(userId, product)
            .Returns(Task.FromException<ProductEntity>(new InvalidArgumentException("Invalid product")));
        
        var result = await _productController.PostProduct(userId, product);
        
        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterPostProductWhenUnhandledExceptionThrown()
    {
        var userId = Guid.NewGuid();
        var product = new Product { Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };

        _productService.PostProduct(userId, product)
            .Returns(Task.FromException<ProductEntity>(new Exception("Unexpected error")));

        var result = await _productController.PostProduct(userId, product);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);
    }

    #endregion

    #region GetProduct Tests

    [Test]
    public async Task ShouldReturnOkAfterGetProduct()
    {
        long productId = 1;
        var productEntity = new ProductEntity
            { Id = productId, Name = "Laptop", Description = "Gaming Laptop", BasePrice = 2500.99m };

        _productService.GetProduct(productId).Returns(productEntity);
        
        var result = await _productController.GetProduct(productId);
        
        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterGetProductWhenProductNotFound()
    {
        long productId = 1;

        _productService.GetProduct(productId)
            .Returns(Task.FromException<ProductEntity>(new NotFoundException("Product not found")));
        
        var result = await _productController.GetProduct(productId);
        
        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnInternalServerErrorAfterGetProductWhenUnhandledExceptionThrown()
    {
        long productId = 1;

        _productService.GetProduct(productId)
            .Returns(Task.FromException<ProductEntity>(new Exception("Unexpected error")));
        
        var result = await _productController.GetProduct(productId);
        
        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<ObjectResult>(result.Result);
    }

    #endregion

    #region GetProductsByUserId Tests

    [Test]
    public async Task ShouldReturnOkAfterGetProductsByUserId()
    {
        var userId = Guid.NewGuid();
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Laptop", BasePrice = 2500.99m, OwnerId = userId },
            new ProductEntity { Id = 2, Name = "Phone", BasePrice = 999.99m, OwnerId = userId }
        };

        _productService.GetProductsByOwnerId(userId).Returns(products);

        var result = await _productController.GetProducts(userId);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<OkObjectResult>(result.Result);
    }

    [Test]
    public async Task ShouldReturnNotFoundAfterGetProductsByUserIdWhenUserNotFound()
    {
        var userId = Guid.NewGuid();

        _productService.GetProductsByOwnerId(userId)
            .Returns(Task.FromException<IEnumerable<ProductEntity>>(new NotFoundException("User not found")));

        var result = await _productController.GetProducts(userId);

        ClassicAssert.IsNotNull(result);
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result.Result);
    }

    #endregion

    #region GetProducts Tests

    [Test]
    public async Task ShouldReturnOkAfterGetProducts()
    {
        var products = new List<ProductEntity>
        {
            new ProductEntity { Id = 1, Name = "Laptop", BasePrice = 2500.99m },
            new ProductEntity { Id = 2, Name = "Phone", BasePrice = 999.99m }
        };

        _productService.GetProducts().Returns(products);

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
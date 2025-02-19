using FluentValidation;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<Product> _productModelValidator;

    public ProductService(IProductRepository productRepository, IUserRepository userRepository,
        IValidator<Product> productModelValidator)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _productModelValidator = productModelValidator;
    }

    public async Task<ProductEntity> PostProduct(Guid userId, Product product)
    {
        var userEntity = await _userRepository.GetUser(userId);
        if (userEntity == null)
        {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        var validationResult = await _productModelValidator.ValidateAsync(product);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new InvalidArgumentException(errors);
        }

        ProductEntity productEntity = new ProductEntity(product)
        {
            OwnerId = userId
        };

        await _productRepository.InsertProductAsync(productEntity);
        await _productRepository.SaveAsync();

        return productEntity;
    }

    public async Task<ProductEntity> GetProduct(long productId)
    {
        var productEntity = await _productRepository.GetProduct(productId);
        if (productEntity == null)
        {
            throw new NotFoundException(ErrorMessages.ProductNotFoundException);
        }

        return productEntity;
    }

    public async Task<IEnumerable<ProductEntity>> GetProducts()
    {
        return await _productRepository.GetProducts();
    }

    public async Task<IEnumerable<ProductEntity>> GetProductsByOwnerId(Guid userId)
    {
        var user = await _userRepository.GetUser(userId);
        if (user == null)
        {
            throw new NotFoundException(ErrorMessages.UserNotFound);
        }

        return await _productRepository.GetProducts(userId);
    }
}
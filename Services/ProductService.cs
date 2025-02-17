﻿using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public ProductService(IProductRepository productRepository, IUserRepository userRepository)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
    }
    public async Task<ProductEntity> PostProduct(Guid userId, Product product)
    {

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
        return await _productRepository.GetProduct(productId);
    }
    
    public async Task<IEnumerable<ProductEntity>> GetProductsByOwnerId(Guid userId)
    {
        await _userRepository.GetUser(userId);
        return await _productRepository.GetProducts(userId);
    }
}
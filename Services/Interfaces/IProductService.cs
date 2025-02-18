using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Services.Interfaces;

public interface IProductService
{
    Task<ProductEntity> PostProduct(Guid userId, Product product);
    Task<ProductEntity> GetProduct(long offerId);
    Task<IEnumerable<ProductEntity>> GetProductsByOwnerId(Guid userId);
    Task<IEnumerable<ProductEntity>> GetProducts();
}
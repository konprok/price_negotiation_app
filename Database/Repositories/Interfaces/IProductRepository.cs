using PriceNegotiationApp.Database.Entities;

namespace PriceNegotiationApp.Database.Repositories.Interfaces;

public interface IProductRepository
{
    Task<ProductEntity?> GetProduct(long productId);
    Task<IEnumerable<ProductEntity>> GetProducts();
    Task<IEnumerable<ProductEntity>> GetProducts(Guid userId);
    Task InsertProductAsync(ProductEntity productEntity);
    Task SaveAsync();
}
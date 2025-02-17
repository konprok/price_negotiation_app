using Microsoft.EntityFrameworkCore;
using PriceNegotiationApp.Database.DbContext;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Models.Exceptions;

namespace PriceNegotiationApp.Database.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductEntity> GetProduct(long productId)
    {
        var product = await _dbContext.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();
        if (product == null)
        {
            throw new ProductNotFoundException();
        }

        return product;
    }

    public async Task<IEnumerable<ProductEntity>> GetProducts(Guid ownerId)
    {
        var products = await _dbContext.Products
            .Where(x => x.OwnerId == ownerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return products;
    }

    public async Task InsertProductAsync(ProductEntity productEntity)
    {
        await _dbContext.Products.AddAsync(productEntity);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
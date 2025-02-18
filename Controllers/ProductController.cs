using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ProductEntity>> PostProduct(Guid userId, [FromBody] Product product)
    {
        try
        {
            return Ok(await _productService.PostProduct(userId, product));
        }
        catch (InvalidInputException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ProductEntity>> GetProduct(long productId)
    {
        try
        {
            return Ok(await _productService.GetProduct(productId));
        }
        catch (ProductNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<ProductEntity>> GetProducts(Guid userId)
    {
        try
        {
            return Ok(await _productService.GetProductsByOwnerId(userId));
        }
        catch (UserNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<ProductEntity>> GetProducts()
    {
        try
        {
            return Ok(await _productService.GetProducts());
        }
        catch (UserNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
}
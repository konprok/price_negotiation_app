using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Database.Repositories.Interfaces;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[Route("negotiations")]
public class NegotiationController : Controller
{
    private readonly IUserService _userService;
    private readonly IProductService _productService;
    private readonly INegotiationService _negotiationService;

    public NegotiationController(IUserService userService, IProductService productService, INegotiationService negotiationService)
    {
        _userService = userService;
        _productService = productService;
        _negotiationService = negotiationService;
    }

    [HttpPost]
    public async Task<ActionResult<NegotiationEntity>> PostNegotiation(Guid clientId, long productId)
    {
        try
        {
            return Ok(await _negotiationService.PostNegotiation(clientId, productId));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("proposition")]
    public async Task<ActionResult<PropositionEntity>> PostProposition(long negotiationId, decimal price)
    {
        try
        {
            return Ok(await _negotiationService.PostProposition(negotiationId, price));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
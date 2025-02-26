using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Models;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Controllers;

[ApiController]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenGeneratorRequest request)
    {
        var jwt = _identityService.GenerateToken(request);
        
        return Ok(new { token = jwt });
    }
}
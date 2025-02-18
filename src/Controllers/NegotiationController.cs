using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[Route("negotiations")]
public class NegotiationController : Controller
{
    private readonly INegotiationService _negotiationService;

    public NegotiationController(INegotiationService negotiationService)
    {
        _negotiationService = negotiationService;
    }

    [HttpPost("proposition")]
    public async Task<ActionResult<PropositionEntity>> PostProposition([FromBody]Guid clientId, [FromBody] long productId, [FromBody] decimal price)
    {
        try
        {
            return Ok(await _negotiationService.PostProposition(clientId, productId, price));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("proposition")]
    public async Task<ActionResult<PropositionEntity>> PatchProposition([FromBody] Guid userId, [FromBody] long negotiationId, [FromBody] bool response)
    {
        try
        {
            return Ok(await _negotiationService.PatchProposition(userId, negotiationId, response));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NegotiationEntity?>>> GetNegotiations(Guid userId)
    {
        try
        {
            return Ok(await _negotiationService.GetNegotiations(userId));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Database.Entities;
using PriceNegotiationApp.Models.Dtos;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Services.Interfaces;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[Authorize]
[Route("negotiations")]
public sealed class NegotiationController : Controller
{
    private readonly INegotiationService _negotiationService;

    public NegotiationController(INegotiationService negotiationService)
    {
        _negotiationService = negotiationService;
    }

    [HttpPost("proposition")]
    public async Task<ActionResult<PropositionEntity>> PostProposition([FromBody] PostPropositionDto postPropositionDto)
    {
        try
        {
            return Ok(await _negotiationService.PostProposition(postPropositionDto.ClientId,
                postPropositionDto.ProductId, postPropositionDto.Price));
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
    public async Task<ActionResult<PropositionEntity>> PatchProposition(
        [FromBody] PatchPropositionDto patchPropositionDto)
    {
        try
        {
            return Ok(await _negotiationService.PatchProposition(patchPropositionDto.UserId,
                patchPropositionDto.NegotiationId, patchPropositionDto.Response));
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
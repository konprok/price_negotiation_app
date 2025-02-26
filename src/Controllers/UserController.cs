using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[AllowAnonymous]
[Route("users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IIdentityService _identityService;

    public UserController(IUserService userService, IIdentityService identityService)
    {
        _userService = userService;
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> PostUser([FromBody] UserRegisterDto user) 
    {
        try
        {
            return Ok(await _userService.PostUser(user));
        }
        catch (InvalidArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> PostUserLogin([FromBody] UserLoginDto user)
    {
        try
        {
            var userResponse = await _userService.GetUser(user);

            var token = _identityService.GenerateToken(userResponse);

            return Ok(new 
            {
                User = userResponse,
                Token = token
            });
        }
        catch (InvalidArgumentException ex)
        {
            return BadRequest(ex.Message);
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
}
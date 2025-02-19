using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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
    public async Task<ActionResult<UserResponse>> PostUserLogin([FromBody] UserLoginDto user)
    {
        try
        {
            return Ok(await _userService.GetUser(user.Email, user.Password));
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
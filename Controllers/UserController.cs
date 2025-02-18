using Microsoft.AspNetCore.Mvc;
using PriceNegotiationApp.Services.Interfaces;
using PriceNegotiationApp.Models.Exceptions;
using PriceNegotiationApp.Models.Dtos;

namespace PriceNegotiationApp.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
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
        catch (InvalidInputException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidUserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UserAlreadyExistException ex)
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
        catch (InvalidUserException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UserNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidPasswordException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using BlogPost.Repository.Models;
using BlogPost.Service.Services;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] UserModel model)
    {
        if (model.UserName is null || model.Password is null)
        {
            return Unauthorized();
        }

        var response = await _userService.Login(model);

        if (response is null)
        {
            return BadRequest("Invalid credentials");
        }
        return Ok(response);
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request");
        }

        var response = await _userService.RefreshToken(tokenModel);
        if (response is null)
        {
            return BadRequest("Invalid access token or refresh token");
        }

        return new ObjectResult(response);
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string userName)
    {
        await _userService.RevokeToken(userName);

        return NoContent();
    }

    [Authorize]
    [HttpPost]
    [Route("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        await _userService.RevokeAllToken();

        return NoContent();
    }
}

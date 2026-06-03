namespace AndrezOG.Api.Rest;

using AndrezOG.Application.Iservices;
using AndrezOG.Api.Rest.Dto.Auth;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }
        return Ok(new {
            message = result.Message,
            token = result.Token,
            email = result.Email,
            role = result.Role
        });

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
        {
            return Unauthorized(new { message = result.Message });
        }
        return Ok(new {
            message = result.Message,
            token = result.Token,
            email = result.Email,
            role = result.Role
        });
    }
}
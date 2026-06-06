namespace AndrezOG.Api.Rest;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Mapper.Auth;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;

    public AuthController(IAuthService authService, IProfileService profileService)
    {
        _authService = authService;
        _profileService = profileService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!AuthMappers.PasswordsMatch(request))
        {
            return BadRequest(new ErrorResponse("Las contraseñas no coinciden"));
        }

        var registerCommand = AuthMappers.ToRegisterCommand(request);
        var result = await _authService.RegisterAsync(registerCommand);

        if (!result.Success)
        {
            return BadRequest(AuthMappers.ToErrorResponse(result));
        }

        var profileCommand = AuthMappers.ToCreateDefaultProfileCommand(request, result.UserId!.Value);
        await _profileService.CreateDefaultProfileAsync(profileCommand);

        return Ok(AuthMappers.ToAuthResponse(result));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _authService.LoginAsync(command);

        if (!result.Success)
        {
            return Unauthorized(AuthMappers.ToErrorResponse(result));
        }

        return Ok(AuthMappers.ToAuthResponse(result));
    }
}

namespace AndrezOG.Api.Rest;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Mapper.Auth;
using AndrezOG.Infrastructure.ContextDb;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly AppDbContext _context;

    public AuthController(IAuthService authService, IProfileService profileService, AppDbContext context)
    {
        _authService = authService;
        _profileService = profileService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!AuthMappers.PasswordsMatch(request))
        {
            return BadRequest(new ErrorResponse("Las contraseñas no coinciden"));
        }

        var registerCommand = AuthMappers.ToRegisterCommand(request);

        // Envolver registro de usuario + creación de perfil en una transacción
        // Si cualquiera de las dos falla, se hace rollback automático
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await _authService.RegisterAsync(registerCommand);

            if (!result.Success)
            {
                await transaction.RollbackAsync();
                return BadRequest(AuthMappers.ToErrorResponse(result));
            }

            var profileCommand = AuthMappers.ToCreateDefaultProfileCommand(request, result.UserId!.Value);
            await _profileService.CreateDefaultProfileAsync(profileCommand);

            await transaction.CommitAsync();
            return Ok(AuthMappers.ToAuthResponse(result));
        }
        catch
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new ErrorResponse("Error al crear el perfil. El registro fue cancelado."));
        }
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

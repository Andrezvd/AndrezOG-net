namespace AndrezOG.Api.Rest;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Mapper.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly IConfiguration _configuration;

    public AuthController(
        IAuthService authService,
        IProfileService profileService,
        IConfiguration configuration)
    {
        _authService = authService;
        _profileService = profileService;
        _configuration = configuration;
    }

    // ================================================================
    // REGISTRO MANUAL
    // ================================================================

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!AuthMappers.PasswordsMatch(request))
        {
            return BadRequest(new ErrorResponse("Las contraseñas no coinciden"));
        }

        var registerCommand = AuthMappers.ToRegisterCommand(request);

        // Primero registrar el usuario
        var result = await _authService.RegisterAsync(registerCommand);

        if (!result.Success)
        {
            return BadRequest(AuthMappers.ToErrorResponse(result));
        }

        // Luego crear el perfil por defecto (si falla, el usuario ya existe pero no tiene perfil)
        try
        {
            var profileCommand = AuthMappers.ToCreateDefaultProfileCommand(request, result.UserId!.Value);
            await _profileService.CreateDefaultProfileAsync(profileCommand);
        }
        catch
        {
            // No deshacer el registro, solo loggear el error
            // El perfil puede crearse después desde el frontend
        }

        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // LOGIN MANUAL
    // ================================================================

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

    // ================================================================
    // GOOGLE OAUTH
    // ================================================================

    /// <summary>
    /// Inicia el flujo OAuth redirigiendo al usuario a Google.
    /// El frontend debe llamar a este endpoint para obtener la URL de Google.
    /// </summary>
    [HttpGet("google/login")]
    public IActionResult GoogleLogin()
    {
        var clientId = _configuration["Google:ClientId"];
        var redirectUri = _configuration["Google:RedirectUri"]
            ?? $"{Request.Scheme}://{Request.Host}/api/auth/google/callback";

        var googleAuthUrl =
            "https://accounts.google.com/o/oauth2/v2/auth" +
            $"?client_id={clientId}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            "&response_type=code" +
            "&scope=openid%20email%20profile" +
            "&access_type=offline" +
            "&prompt=consent";

        return Ok(new { url = googleAuthUrl });
    }

    /// <summary>
    /// Callback que Google llama después de que el usuario se autentica.
    /// Intercambia el code por tokens y crea/linkea el usuario en BD.
    /// </summary>
    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new ErrorResponse("Código de autorización no proporcionado."));
        }

        var redirectUri = _configuration["Google:RedirectUri"]
            ?? $"{Request.Scheme}://{Request.Host}/api/auth/google/callback";

        var command = new GoogleLoginCommand(code, redirectUri);
        var result = await _authService.GoogleLoginAsync(command);

        if (!result.Success)
        {
            return BadRequest(AuthMappers.ToErrorResponse(result));
        }

        // Crear perfil por defecto si es un usuario nuevo (sin GoogleId preexistente)
        // El AuthResult ya tiene Name del userinfo de Google
        if (!string.IsNullOrWhiteSpace(result.Name))
        {
            // Intentar crear perfil; si ya existe, no pasa nada
            try
            {
                var profileCommand = new CreateDefaultProfileCommand(
                    result.UserId!.Value,
                    result.Email!,
                    result.Name,
                    string.Empty,
                    string.Empty,
                    string.Empty
                );
                await _profileService.CreateDefaultProfileAsync(profileCommand);
            }
            catch
            {
                // El perfil probablemente ya existe, ignorar
            }
        }

        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // REFRESH TOKEN
    // ================================================================

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _authService.RefreshTokenAsync(command);

        if (!result.Success)
        {
            return Unauthorized(AuthMappers.ToErrorResponse(result));
        }

        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // LOGOUT
    // ================================================================

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(new ErrorResponse("No se pudo identificar al usuario."));
        }

        await _authService.LogoutAsync(userId);
        return Ok(new { message = "Sesión cerrada exitosamente." });
    }
}
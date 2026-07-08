namespace AndrezOG.Api.Rest;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Mapper.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        IAuthService authService,
        IProfileService profileService,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _profileService = profileService;
        _environment = environment;
    }

    // ================================================================
    // REGISTRO MANUAL
    // ================================================================

    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!AuthMappers.PasswordsMatch(request))
        {
            return BadRequest(new ErrorResponse("Las contraseñas no coinciden"));
        }

        var userCommand = AuthMappers.ToRegisterCommand(request);
        var profileCommand = AuthMappers.ToCreateDefaultProfileCommand(request, 0);
        var result = await _authService.RegisterWithProfileAsync(userCommand, profileCommand);

        if (!result.Success)
        {
            return BadRequest(AuthMappers.ToErrorResponse(result));
        }

        SetRefreshTokenCookie(result.RefreshToken!);
        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // LOGIN MANUAL
    // ================================================================

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _authService.LoginAsync(command);

        if (!result.Success)
        {
            return Unauthorized(AuthMappers.ToErrorResponse(result));
        }

        SetRefreshTokenCookie(result.RefreshToken!);

        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // LOGIN EXTERNO (Google, GitHub, etc.)
    // ================================================================

    [HttpPost("external")]
    [EnableRateLimiting("register")]
    public async Task<ActionResult<AuthResponse>> ExternalLogin([FromBody] ExternalLoginRequest request)
    {
        var command = new ExternalLoginCommand(request.Provider, request.IdToken);
        var result = await _authService.ExternalLoginAsync(command);

        if (!result.Success)
        {
            return BadRequest(AuthMappers.ToErrorResponse(result));
        }

        // Crear perfil por defecto (nombre y apellido de Google separados)
        try
        {
            var profileCommand = new CreateDefaultProfileCommand(
                result.UserId!.Value,
                result.Email!,
                result.Name ?? string.Empty,
                result.LastName ?? string.Empty,
                string.Empty,
                string.Empty
            );
            await _profileService.CreateDefaultProfileAsync(profileCommand);
        }
        catch { /* perfil ya existe */ }

        SetRefreshTokenCookie(result.RefreshToken!);
        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // REFRESH TOKEN (desde cookie HttpOnly)
    // ================================================================

    [HttpPost("refresh")]
    [EnableRateLimiting("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized(new ErrorResponse("No se encontró refresh token."));
        }

        var result = await _authService.RefreshTokenAsync(refreshToken);

        if (!result.Success)
        {
            return Unauthorized(AuthMappers.ToErrorResponse(result));
        }

        // Rotar refresh token
        SetRefreshTokenCookie(result.RefreshToken!);
        return Ok(AuthMappers.ToAuthResponse(result));
    }

    // ================================================================
    // LOGOUT
    // ================================================================

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(new ErrorResponse("No se pudo identificar al usuario."));
        }

        await _authService.LogoutAsync(userId);
        Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/api/auth",
            SameSite = SameSiteMode.Strict,
            Secure = !_environment.IsDevelopment(),
            HttpOnly = true
        });
        return Ok(new { message = "Sesión cerrada exitosamente." });
    }

    // ================================================================
    // EMAIL VERIFICATION
    // ================================================================

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new ErrorResponse("Token de verificación no proporcionado."));
        }

        var result = await _authService.VerifyEmailAsync(token);

        if (!result.Success)
        {
            return BadRequest(AuthMappers.ToErrorResponse(result));
        }

        return Ok(new { message = result.Message });
    }

    // ================================================================
    // HELPERS
    // ================================================================

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/api/auth"
        });
    }
}
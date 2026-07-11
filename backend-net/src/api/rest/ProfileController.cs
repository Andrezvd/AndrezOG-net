namespace AndrezOG.Api.Rest;

using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Dto.Profile;
using AndrezOG.Api.Rest.Mapper.Profile;
using AndrezOG.Application.Iservices;
using AndrezOG.Shared.StorageService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IFileStorageService _fileStorage;

    public ProfileController(IProfileService profileService, IFileStorageService fileStorage)
    {
        _profileService = profileService;
        _fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublicProfile()
    {
        var profile = await _profileService.GetPublicProfileAsync();
        if (profile is null)
        {
            return NotFound(new ErrorResponse("Perfil no encontrado"));
        }
        return Ok(ProfileMapper.DomainToDto(profile, _fileStorage));
    }

    [HttpPatch("me")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("No se pudo identificar al usuario."));
        }

        var command = ProfileMapper.ToUpdateProfileCommand(userId, request);

        try
        {
            await _profileService.UpdateProfileAsync(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse("Perfil no encontrado"));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("No se pudo identificar al usuario."));
        }

        var profile = await _profileService.GetByUserIdAsync(userId);
        if (profile is null)
        {
            return NotFound(new ErrorResponse("Perfil no encontrado"));
        }
        return Ok(ProfileMapper.DomainToDto(profile, _fileStorage));
    }

    [HttpPost("photo")]
    [Authorize]
    [EnableRateLimiting("upload")]
    public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new ErrorResponse("No se pudo identificar al usuario."));
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new ErrorResponse("Archivo no proporcionado."));
        }

        string photoUrl;
        try
        {
            photoUrl = await _fileStorage.SaveFileAsync(file, "profiles");
        }
        catch (Exception)
        {
            return BadRequest(new ErrorResponse("Error al procesar el archivo. Verifica que sea una imagen válida."));
        }

        var profile = await _profileService.GetByUserIdAsync(userId);
        if (profile is null)
        {
            return NotFound(new ErrorResponse("Perfil no encontrado"));
        }

        // Eliminar foto anterior si existe
        await _fileStorage.DeleteFileAsync(profile.PhotoUrl);

        // Actualizar el perfil con la nueva URL de la foto
        var command = new Application.Commands.UpdateProfileCommand(
            userId, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, photoUrl, null, null, null, null
        );
        await _profileService.UpdateProfileAsync(command);

        return Ok(new { photoUrl = _fileStorage.GetPublicUrl(photoUrl) });
    }
}

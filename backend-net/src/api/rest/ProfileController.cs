namespace AndrezOG.Api.Rest;

using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Dto.Profile;
using AndrezOG.Api.Rest.Mapper.Profile;
using AndrezOG.Application.Iservices;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPublicProfile()
    {
        var profile = await _profileService.GetPublicProfileAsync();
        if (profile is null)
        {
            return NotFound(new ErrorResponse("Perfil no encontrado"));
        }
        return Ok(ProfileMapper.DomainToDto(profile));
    }

    [HttpPatch("{userId:int}")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateRequest request)
    {
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
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _profileService.GetMyProfileAsync();
        if (profile is null)
        {
            return NotFound(new ErrorResponse("Perfil no encontrado"));
        }
        return Ok(ProfileMapper.DomainToDto(profile));
    }
}
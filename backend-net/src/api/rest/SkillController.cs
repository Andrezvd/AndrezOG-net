namespace AndrezOG.Api.Rest;

using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Dto.Skill;
using AndrezOG.Api.Rest.Mapper.Skill;
using AndrezOG.Application.Iservices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    // =========================================
    // ENDPOINTS PUBLICOS (sin autenticacion)
    // =========================================

    /// <summary>
    /// Vitrina publica: lista de skills activas con id, name, imageUrl.
    /// </summary>
    [HttpGet("public")]
    public async Task<IActionResult> GetPublicSkills()
    {
        var result = await _skillService.GetActiveSkillsAsync();
        if (!result.Success)
        {
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        var cards = result.Skills!
            .Select(SkillMappers.ToCardDto)
            .ToList();

        return Ok(cards);
    }

    /// <summary>
    /// Retorna la URL de la imagen de una skill por su ID.
    /// </summary>
    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> GetSkillImage(int id)
    {
        var result = await _skillService.GetSkillImageByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ErrorResponse(result.ErrorMessage));
        }

        return Ok(new SkillImageResponseDto { ImageUrl = result.ImageUrl });
    }

    // =========================================
    // ENDPOINTS DE ADMINISTRACION (autenticados)
    // =========================================

    /// <summary>
    /// Panel admin: listar todas las skills (activas e inactivas).
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllSkills()
    {
        var result = await _skillService.GetAllSkillsAsync();
        if (!result.Success)
        {
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        var response = result.Skills!
            .Select(SkillMappers.ToResponseDto)
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Panel admin: obtener una skill por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetSkillById(int id)
    {
        var result = await _skillService.GetSkillByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(new ErrorResponse(result.ErrorMessage));
        }

        return Ok(SkillMappers.ToResponseDto(result.Skill!));
    }

    /// <summary>
    /// Selector/asignacion: lista de skills activas con id y name.
    /// </summary>
    [HttpGet("options")]
    [Authorize]
    public async Task<IActionResult> GetSkillOptions()
    {
        var result = await _skillService.GetActiveSkillsAsync();
        if (!result.Success)
        {
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        var options = result.Skills!
            .Select(SkillMappers.ToOptionDto)
            .ToList();

        return Ok(options);
    }

    /// <summary>
    /// Panel admin: crear una nueva skill.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateSkill([FromForm] SkillRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = SkillMappers.ToCreateCommand(request);
        var result = await _skillService.CreateSkillAsync(command);

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        return CreatedAtAction(
            nameof(GetSkillById),
            new { id = result.Skill!.Id },
            SkillMappers.ToResponseDto(result.Skill));
    }

    /// <summary>
    /// Panel admin: actualizar una skill existente.
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateSkill(int id, [FromForm] UpdateSkillRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = SkillMappers.ToUpdateCommand(id, request);
        var result = await _skillService.UpdateSkillAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new ErrorResponse(result.ErrorMessage));
            }
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        return Ok(SkillMappers.ToResponseDto(result.Skill!));
    }

    /// <summary>
    /// Panel admin: soft delete (marca IsActive = false).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteSkill(int id)
    {
        var result = await _skillService.SoftDeleteSkillAsync(id);
        if (!result.Success)
        {
            return NotFound(new ErrorResponse(result.ErrorMessage));
        }

        return NoContent();
    }

    /// <summary>
    /// Panel admin: hard delete (eliminacion fisica + archivo de imagen).
    /// </summary>
    [HttpDelete("{id:int}/hard")]
    [Authorize]
    public async Task<IActionResult> HardDeleteSkill(int id)
    {
        var result = await _skillService.HardDeleteSkillAsync(id);
        if (!result.Success)
        {
            return NotFound(new ErrorResponse(result.ErrorMessage));
        }

        return NoContent();
    }
}

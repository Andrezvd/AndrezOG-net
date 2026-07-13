namespace AndrezOG.Api.Rest;

using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Dto.Project;
using AndrezOG.Api.Rest.Mapper.Project;
using AndrezOG.Application.Iservices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // =========================================
    // ENDPOINTS PUBLICOS (sin autenticacion)
    // =========================================

    /// <summary>
    /// Vitrina publica: lista de proyectos activos con sus stacks y skills.
    /// </summary>
    [HttpGet("public")]
    [EnableRateLimiting("public")]
    public async Task<IActionResult> GetPublicProjects()
    {
        var result = await _projectService.GetActiveProjectsAsync();
        if (!result.Success)
            return BadRequest(new ErrorResponse(result.ErrorMessage));

        var cards = result.Projects!
            .Select(ProjectMappers.ToCardDto)
            .ToList();

        return Ok(cards);
    }

    // =========================================
    // ENDPOINTS DE ADMINISTRACION (autenticados)
    // =========================================

    /// <summary>
    /// Panel admin: listar todos los proyectos (activos e inactivos).
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllProjects()
    {
        var result = await _projectService.GetAllProjectsAsync();
        if (!result.Success)
            return BadRequest(new ErrorResponse(result.ErrorMessage));

        var response = result.Projects!
            .Select(ProjectMappers.ToResponseDto)
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Panel admin: obtener un proyecto por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var result = await _projectService.GetProjectByIdAsync(id);
        if (!result.Success)
            return NotFound(new ErrorResponse(result.ErrorMessage));

        return Ok(ProjectMappers.ToResponseDto(result.Project!));
    }

    /// <summary>
    /// Panel admin: crear un nuevo proyecto.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProject([FromForm] ProjectRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = ProjectMappers.ToCreateCommand(request);
        var result = await _projectService.CreateProjectAsync(command);

        if (!result.Success)
            return BadRequest(new ErrorResponse(result.ErrorMessage));

        return CreatedAtAction(
            nameof(GetProjectById),
            new { id = result.Project!.Id },
            ProjectMappers.ToResponseDto(result.Project));
    }

    /// <summary>
    /// Panel admin: actualizar un proyecto existente.
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateProject(int id, [FromForm] UpdateProjectRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = ProjectMappers.ToUpdateCommand(id, request);
        var result = await _projectService.UpdateProjectAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(new ErrorResponse(result.ErrorMessage));
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        return Ok(ProjectMappers.ToResponseDto(result.Project!));
    }

    /// <summary>
    /// Panel admin: soft delete (marca IsActive = false).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteProject(int id)
    {
        var result = await _projectService.SoftDeleteProjectAsync(id);
        if (!result.Success)
            return NotFound(new ErrorResponse(result.ErrorMessage));

        return NoContent();
    }

    /// <summary>
    /// Panel admin: hard delete (eliminacion fisica).
    /// </summary>
    [HttpDelete("{id:int}/hard")]
    [Authorize]
    public async Task<IActionResult> HardDeleteProject(int id)
    {
        var result = await _projectService.HardDeleteProjectAsync(id);
        if (!result.Success)
            return NotFound(new ErrorResponse(result.ErrorMessage));

        return NoContent();
    }
}
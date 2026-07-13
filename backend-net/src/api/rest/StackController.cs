namespace AndrezOG.Api.Rest;

using AndrezOG.Api.Rest.Dto.Auth;
using AndrezOG.Api.Rest.Dto.Stack;
using AndrezOG.Api.Rest.Mapper.Stack;
using AndrezOG.Application.Iservices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/[controller]")]
public class StackController : ControllerBase
{
    private readonly IStackService _stackService;

    public StackController(IStackService stackService)
    {
        _stackService = stackService;
    }

    // =========================================
    // ENDPOINTS PUBLICOS (sin autenticacion)
    // =========================================

    /// <summary>
    /// Vitrina publica: lista de stacks activos con sus skills.
    /// </summary>
    [HttpGet("public")]
    [EnableRateLimiting("public")]
    public async Task<IActionResult> GetPublicStacks()
    {
        var result = await _stackService.GetActiveStacksAsync();
        if (!result.Success)
            return BadRequest(new ErrorResponse(result.ErrorMessage));

        var cards = result.Stacks!
            .Select(StackMappers.ToCardDto)
            .ToList();

        return Ok(cards);
    }

    // =========================================
    // ENDPOINTS DE ADMINISTRACION (autenticados)
    // =========================================

    /// <summary>
    /// Panel admin: listar todos los stacks (activos e inactivos).
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllStacks()
    {
        var result = await _stackService.GetAllStacksAsync();
        if (!result.Success)
            return BadRequest(new ErrorResponse(result.ErrorMessage));

        var response = result.Stacks!
            .Select(StackMappers.ToResponseDto)
            .ToList();

        return Ok(response);
    }

    /// <summary>
    /// Panel admin: obtener un stack por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetStackById(int id)
    {
        var result = await _stackService.GetStackByIdAsync(id);
        if (!result.Success)
            return NotFound(new ErrorResponse(result.ErrorMessage));

        return Ok(StackMappers.ToResponseDto(result.Stack!));
    }

    /// <summary>
    /// Panel admin: crear un nuevo stack.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateStack([FromBody] StackRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = StackMappers.ToCreateCommand(request);
        var result = await _stackService.CreateStackAsync(command);

        if (!result.Success)
            return BadRequest(new ErrorResponse(result.ErrorMessage));

        return CreatedAtAction(
            nameof(GetStackById),
            new { id = result.Stack!.Id },
            StackMappers.ToResponseDto(result.Stack));
    }

    /// <summary>
    /// Panel admin: actualizar un stack existente.
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateStack(int id, [FromBody] UpdateStackRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = StackMappers.ToUpdateCommand(id, request);
        var result = await _stackService.UpdateStackAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(new ErrorResponse(result.ErrorMessage));
            return BadRequest(new ErrorResponse(result.ErrorMessage));
        }

        return Ok(StackMappers.ToResponseDto(result.Stack!));
    }

    /// <summary>
    /// Panel admin: soft delete (marca IsActive = false).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteStack(int id)
    {
        var result = await _stackService.SoftDeleteStackAsync(id);
        if (!result.Success)
            return NotFound(new ErrorResponse(result.ErrorMessage));

        return NoContent();
    }

    /// <summary>
    /// Panel admin: hard delete (eliminacion fisica).
    /// </summary>
    [HttpDelete("{id:int}/hard")]
    [Authorize]
    public async Task<IActionResult> HardDeleteStack(int id)
    {
        var result = await _stackService.HardDeleteStackAsync(id);
        if (!result.Success)
            return NotFound(new ErrorResponse(result.ErrorMessage));

        return NoContent();
    }
}
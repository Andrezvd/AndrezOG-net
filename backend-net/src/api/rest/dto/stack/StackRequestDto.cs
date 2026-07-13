namespace AndrezOG.Api.Rest.Dto.Stack;

using System.ComponentModel.DataAnnotations;

public class StackRequestDto
{
    [Required(ErrorMessage = "El resumen del stack es obligatorio.")]
    [MaxLength(500)]
    public string Summary { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public List<int> SkillIds { get; set; } = new();
}

public class UpdateStackRequestDto
{
    [Required(ErrorMessage = "El resumen del stack es obligatorio.")]
    [MaxLength(500)]
    public string Summary { get; set; } = string.Empty;

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public List<int> SkillIds { get; set; } = new();
}
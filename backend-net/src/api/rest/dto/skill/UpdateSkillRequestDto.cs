namespace AndrezOG.Api.Rest.Dto.Skill;

using System.ComponentModel.DataAnnotations;
using AndrezOG.Domain.Model.Skills;

public class UpdateSkillRequestDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de skill es requerido")]
    public SkillType SkillType { get; set; }

    [MaxLength(500, ErrorMessage = "La descripcion no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public IFormFile? ImageFile { get; set; }

    public bool RemoveImage { get; set; } = false;
}

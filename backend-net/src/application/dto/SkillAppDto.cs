namespace AndrezOG.Application.Dto;

/// <summary>
/// DTO de capa de aplicacion.
/// Representa una skill tal como el servicio la expone a las capas superiores.
/// Aisla el modelo de dominio (Domain.Model.Skills.Skill) de la capa REST.
/// </summary>
public class SkillAppDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SkillType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
}
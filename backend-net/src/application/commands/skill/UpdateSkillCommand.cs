namespace AndrezOG.Application.Commands.Skill;

using AndrezOG.Domain.Model.Skills;

public record UpdateSkillCommand(
    int Id,
    string Name,
    SkillType SkillType,
    string? Description,
    bool IsActive,
    IFormFile? ImageFile,
    bool RemoveImage
);
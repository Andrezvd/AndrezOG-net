namespace AndrezOG.Application.Commands.Skill;

using AndrezOG.Domain.Model.Skills;

public record CreateSkillCommand(
    string Name,
    SkillType SkillType,
    string? Description,
    bool IsActive,
    IFormFile? ImageFile
);
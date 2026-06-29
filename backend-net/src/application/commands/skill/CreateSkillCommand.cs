namespace AndrezOG.Application.Commands.Skill;

public record CreateSkillCommand(
    string Name,
    string SkillType,
    string? Description,
    bool IsActive,
    IFormFile? ImageFile
);

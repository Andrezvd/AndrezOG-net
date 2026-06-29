namespace AndrezOG.Application.Commands.Skill;

public record UpdateSkillCommand(
    int Id,
    string Name,
    string SkillType,
    string? Description,
    bool IsActive,
    IFormFile? ImageFile,
    bool RemoveImage
);

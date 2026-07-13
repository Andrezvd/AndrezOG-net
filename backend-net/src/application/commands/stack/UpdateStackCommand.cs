namespace AndrezOG.Application.Commands.Stack;

public record UpdateStackCommand(
    int Id,
    string Summary,
    string Category,
    bool IsActive,
    List<int> SkillIds
);
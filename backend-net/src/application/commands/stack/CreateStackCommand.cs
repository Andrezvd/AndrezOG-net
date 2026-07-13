namespace AndrezOG.Application.Commands.Stack;

public record CreateStackCommand(
    string Summary,
    string Category,
    bool IsActive,
    List<int> SkillIds
);
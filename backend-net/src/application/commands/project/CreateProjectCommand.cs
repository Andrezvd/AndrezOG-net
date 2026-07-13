namespace AndrezOG.Application.Commands.Project;

using AndrezOG.Domain.Model.StackProject;

public record CreateProjectCommand(
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    string? RepositoryUrl,
    ProjectType Type,
    IFormFile? ImageFile,
    List<int> StackIds
);
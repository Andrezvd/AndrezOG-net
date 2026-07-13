namespace AndrezOG.Application.Commands.Project;

using AndrezOG.Domain.Model.StackProject;

public record UpdateProjectCommand(
    int Id,
    string Title,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    string? RepositoryUrl,
    ProjectType Type,
    IFormFile? ImageFile,
    bool RemoveImage,
    List<int> StackIds
);
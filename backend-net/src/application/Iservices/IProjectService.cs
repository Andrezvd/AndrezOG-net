namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands.Project;
using AndrezOG.Application.Result;

public interface IProjectService
{
    // Lectura
    Task<ProjectResult> GetProjectByIdAsync(int id);
    Task<ProjectResult> GetAllProjectsAsync();
    Task<ProjectResult> GetActiveProjectsAsync();

    // Escritura
    Task<ProjectResult> CreateProjectAsync(CreateProjectCommand command);
    Task<ProjectResult> UpdateProjectAsync(UpdateProjectCommand command);
    Task<ProjectResult> SoftDeleteProjectAsync(int id);
    Task<ProjectResult> HardDeleteProjectAsync(int id);
}
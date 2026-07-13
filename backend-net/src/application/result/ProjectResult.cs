namespace AndrezOG.Application.Result;

using AndrezOG.Application.Dto.Project;

public class ProjectResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public ProjectAppDto? Project { get; set; }
    public List<ProjectAppDto>? Projects { get; set; }

    public static ProjectResult Ok(ProjectAppDto project) =>
        new() { Success = true, Project = project };

    public static ProjectResult OkList(List<ProjectAppDto> projects) =>
        new() { Success = true, Projects = projects };

    public static ProjectResult Fail(string message) =>
        new() { Success = false, ErrorMessage = message };
}
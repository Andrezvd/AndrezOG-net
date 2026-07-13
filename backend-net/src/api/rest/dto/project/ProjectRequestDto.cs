namespace AndrezOG.Api.Rest.Dto.Project;

using AndrezOG.Domain.Model.StackProject;
using System.ComponentModel.DataAnnotations;

public class ProjectRequestDto
{
    [Required(ErrorMessage = "El título del proyecto es obligatorio.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(6);

    public bool IsActive { get; set; } = true;

    public string? RepositoryUrl { get; set; }

    public ProjectType Type { get; set; } = ProjectType.Personal;

    public IFormFile? ImageFile { get; set; }

    public List<int> StackIds { get; set; } = new();
}

public class UpdateProjectRequestDto
{
    [Required(ErrorMessage = "El título del proyecto es obligatorio.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? RepositoryUrl { get; set; }

    public ProjectType Type { get; set; } = ProjectType.Personal;

    public IFormFile? ImageFile { get; set; }

    public bool RemoveImage { get; set; }

    public List<int> StackIds { get; set; } = new();
}
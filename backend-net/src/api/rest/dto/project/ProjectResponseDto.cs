namespace AndrezOG.Api.Rest.Dto.Project;

using AndrezOG.Api.Rest.Dto.Stack;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? RepositoryUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<StackCardDto> Stacks { get; set; } = new();
}

public class ProjectCardResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<StackCardDto> Stacks { get; set; } = new();
}
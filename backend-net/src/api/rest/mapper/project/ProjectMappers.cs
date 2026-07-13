namespace AndrezOG.Api.Rest.Mapper.Project;

using AndrezOG.Api.Rest.Dto.Project;
using AndrezOG.Api.Rest.Dto.Stack;
using AndrezOG.Application.Commands.Project;
using AndrezOG.Application.Dto.Project;

public static class ProjectMappers
{
    // Request DTO -> Command
    public static CreateProjectCommand ToCreateCommand(ProjectRequestDto dto) =>
        new(dto.Title, dto.Description, dto.StartDate, dto.EndDate,
            dto.IsActive, dto.RepositoryUrl, dto.Type, dto.ImageFile, dto.StackIds);

    public static UpdateProjectCommand ToUpdateCommand(int id, UpdateProjectRequestDto dto) =>
        new(id, dto.Title, dto.Description, dto.StartDate, dto.EndDate,
            dto.IsActive, dto.RepositoryUrl, dto.Type, dto.ImageFile,
            dto.RemoveImage, dto.StackIds);

    // AppDto -> Response DTO
    public static ProjectResponseDto ToResponseDto(ProjectAppDto project) =>
        new()
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            RepositoryUrl = project.RepositoryUrl,
            Type = project.Type,
            ImageUrl = project.ImageUrl,
            Stacks = project.Stacks.Select(ToStackCard).ToList()
        };

    // AppDto -> Card DTO (público)
    public static ProjectCardResponseDto ToCardDto(ProjectAppDto project) =>
        new()
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl,
            RepositoryUrl = project.RepositoryUrl,
            Type = project.Type,
            Stacks = project.Stacks.Select(ToStackCard).ToList()
        };

    private static StackCardDto ToStackCard(StackSummaryAppDto stack) =>
        new()
        {
            Id = stack.Id,
            Summary = stack.Summary,
            Category = stack.Category,
            Skills = stack.Skills.Select(s => new SkillRefResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                ImageUrl = s.ImageUrl
            }).ToList()
        };
}
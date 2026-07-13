namespace AndrezOG.Api.Rest.Mapper.Stack;

using AndrezOG.Api.Rest.Dto.Stack;
using AndrezOG.Application.Commands.Stack;
using AndrezOG.Application.Dto.Stack;

public static class StackMappers
{
    // Request DTO -> Command
    public static CreateStackCommand ToCreateCommand(StackRequestDto dto) =>
        new(dto.Summary, dto.Category, dto.IsActive, dto.SkillIds);

    public static UpdateStackCommand ToUpdateCommand(int id, UpdateStackRequestDto dto) =>
        new(id, dto.Summary, dto.Category, dto.IsActive, dto.SkillIds);

    // AppDto -> Response DTO
    public static StackResponseDto ToResponseDto(StackAppDto stack) =>
        new()
        {
            Id = stack.Id,
            Summary = stack.Summary,
            Category = stack.Category,
            IsActive = stack.IsActive,
            CreatedAt = stack.CreatedAt,
            UpdatedAt = stack.UpdatedAt,
            Skills = stack.Skills.Select(ToSkillRef).ToList()
        };

    // AppDto -> Card DTO (público)
    public static StackCardDto ToCardDto(StackAppDto stack) =>
        new()
        {
            Id = stack.Id,
            Summary = stack.Summary,
            Category = stack.Category,
            Skills = stack.Skills.Select(ToSkillRef).ToList()
        };

    private static SkillRefResponseDto ToSkillRef(SkillRefAppDto skill) =>
        new()
        {
            Id = skill.Id,
            Name = skill.Name,
            ImageUrl = skill.ImageUrl
        };
}
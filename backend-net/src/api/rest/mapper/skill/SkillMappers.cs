namespace AndrezOG.Api.Rest.Mapper.Skill;

using AndrezOG.Api.Rest.Dto.Skill;
using AndrezOG.Application.Commands.Skill;
using AndrezOG.Application.Dto;

public static class SkillMappers
{
    // Request DTO -> Command
    public static CreateSkillCommand ToCreateCommand(SkillRequestDto dto) =>
        new(dto.Name, dto.SkillType, dto.Description, dto.IsActive, dto.ImageFile);

    public static UpdateSkillCommand ToUpdateCommand(int id, UpdateSkillRequestDto dto) =>
        new(id, dto.Name, dto.SkillType, dto.Description, dto.IsActive, dto.ImageFile, dto.RemoveImage);

    // AppDto -> Response DTO (admin, full detail)
    public static SkillResponseDto ToResponseDto(SkillAppDto skill) =>
        new()
        {
            Id = skill.Id,
            Name = skill.Name,
            SkillType = skill.SkillType,
            Description = skill.Description,
            IsActive = skill.IsActive,
            ImageUrl = skill.ImageUrl
        };

    // AppDto -> Card DTO (vitrina publica: name, imageUrl)
    public static SkillCardDto ToCardDto(SkillAppDto skill) =>
        new()
        {
            Name = skill.Name,
            ImageUrl = skill.ImageUrl
        };

    // AppDto -> Option DTO (selector/dropdown: id, name)
    public static SkillOptionDto ToOptionDto(SkillAppDto skill) =>
        new()
        {
            Id = skill.Id,
            Name = skill.Name
        };

    // AppDto -> Image DTO
    public static SkillImageResponseDto ToImageDto(SkillAppDto skill) =>
        new()
        {
            ImageUrl = skill.ImageUrl ?? string.Empty
        };
}

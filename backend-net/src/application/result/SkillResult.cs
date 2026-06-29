namespace AndrezOG.Application.Result;

using AndrezOG.Application.Dto;

public class SkillResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public SkillAppDto? Skill { get; set; }
    public List<SkillAppDto>? Skills { get; set; }
    public string? ImageUrl { get; set; }

    public static SkillResult Ok(SkillAppDto skill) =>
        new()
        {
            Success = true,
            Skill = skill
        };

    public static SkillResult OkList(List<SkillAppDto> skills) =>
        new()
        {
            Success = true,
            Skills = skills
        };

    public static SkillResult OkImage(string imageUrl) =>
        new()
        {
            Success = true,
            ImageUrl = imageUrl
        };

    public static SkillResult Fail(string message) =>
        new()
        {
            Success = false,
            ErrorMessage = message
        };
}

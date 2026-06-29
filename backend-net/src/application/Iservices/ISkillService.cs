namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands.Skill;
using AndrezOG.Application.Result;

public interface ISkillService
{
    // Lectura
    Task<SkillResult> GetSkillByIdAsync(int id);
    Task<SkillResult> GetAllSkillsAsync();
    Task<SkillResult> GetActiveSkillsAsync();
    Task<SkillResult> GetSkillImageByIdAsync(int id);

    // Escritura
    Task<SkillResult> CreateSkillAsync(CreateSkillCommand command);
    Task<SkillResult> UpdateSkillAsync(UpdateSkillCommand command);
    Task<SkillResult> SoftDeleteSkillAsync(int id);
    Task<SkillResult> HardDeleteSkillAsync(int id);
}

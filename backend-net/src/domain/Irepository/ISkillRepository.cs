namespace AndrezOG.Domain.Irepository;

using AndrezOG.Domain.Model.Skills;

public interface ISkillRepository
{
    // Lectura
    Task<Skill?> GetByIdAsync(int id);
    Task<List<Skill>> ListAllAsync();
    Task<List<Skill>> ListActiveAsync();
    Task<Skill?> GetSkillImageByIdAsync(int id);

    // Escritura
    Task<Skill> CreateAsync(Skill skill);
    Task<Skill?> UpdateAsync(Skill skill);
    Task<Skill?> SoftDeleteAsync(int id);
    Task<Skill?> HardDeleteAsync(int id);

    // Validacion
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}

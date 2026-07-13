namespace AndrezOG.Domain.Irepository;

using AndrezOG.Domain.Model.StackProject;

public interface IProjectRepository
{
    // Lectura
    Task<Project?> GetByIdAsync(int id);
    Task<List<Project>> ListAllAsync();
    Task<List<Project>> ListActiveAsync();

    // Escritura
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(Project project);
    Task<Project?> SoftDeleteAsync(int id);
    Task<Project?> HardDeleteAsync(int id);

    // Validación
    Task<bool> ExistsByTitleAsync(string title, int? excludeId = null);
}
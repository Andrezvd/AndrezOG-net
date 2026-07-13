namespace AndrezOG.Domain.Irepository;

using AndrezOG.Domain.Model.StackProject;

public interface IStackRepository
{
    // Lectura
    Task<Stack?> GetByIdAsync(int id);
    Task<List<Stack>> ListAllAsync();
    Task<List<Stack>> ListActiveAsync();

    // Escritura
    Task<Stack> CreateAsync(Stack stack);
    Task<Stack?> UpdateAsync(Stack stack);
    Task<Stack?> SoftDeleteAsync(int id);
    Task<Stack?> HardDeleteAsync(int id);

    // Validación
    Task<bool> ExistsBySummaryAsync(string summary, int? excludeId = null);
}
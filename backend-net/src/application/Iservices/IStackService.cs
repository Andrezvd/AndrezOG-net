namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands.Stack;
using AndrezOG.Application.Result;

public interface IStackService
{
    // Lectura
    Task<StackResult> GetStackByIdAsync(int id);
    Task<StackResult> GetAllStacksAsync();
    Task<StackResult> GetActiveStacksAsync();

    // Escritura
    Task<StackResult> CreateStackAsync(CreateStackCommand command);
    Task<StackResult> UpdateStackAsync(UpdateStackCommand command);
    Task<StackResult> SoftDeleteStackAsync(int id);
    Task<StackResult> HardDeleteStackAsync(int id);
}
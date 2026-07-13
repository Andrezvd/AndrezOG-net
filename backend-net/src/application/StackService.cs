namespace AndrezOG.Application;

using AndrezOG.Application.Commands.Stack;
using AndrezOG.Application.Dto.Stack;
using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.StackProject;
using AndrezOG.Infrastructure.ContextDb;
using AndrezOG.Shared.StorageService;
using Microsoft.EntityFrameworkCore;

public class StackService : IStackService
{
    private readonly IStackRepository _repository;
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public StackService(IStackRepository repository, AppDbContext context, IFileStorageService fileStorage)
    {
        _repository = repository;
        _context = context;
        _fileStorage = fileStorage;
    }

    // ---------- Lectura ----------

    public async Task<StackResult> GetStackByIdAsync(int id)
    {
        var stack = await _repository.GetByIdAsync(id);
        if (stack is null)
            return StackResult.Fail($"No stack found with ID {id}.");

        return StackResult.Ok(MapToDto(stack));
    }

    public async Task<StackResult> GetAllStacksAsync()
    {
        var stacks = await _repository.ListAllAsync();
        return StackResult.OkList(stacks.Select(MapToDto).ToList());
    }

    public async Task<StackResult> GetActiveStacksAsync()
    {
        var stacks = await _repository.ListActiveAsync();
        return StackResult.OkList(stacks.Select(MapToDto).ToList());
    }

    // ---------- Escritura ----------

    public async Task<StackResult> CreateStackAsync(CreateStackCommand command)
    {
        if (await _repository.ExistsBySummaryAsync(command.Summary))
            return StackResult.Fail($"Ya existe un stack con el resumen '{command.Summary}'.");

        var now = DateTime.UtcNow;

        var stack = new Stack
        {
            Summary = command.Summary,
            Category = command.Category,
            IsActive = command.IsActive,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Asociar skills si se enviaron
        if (command.SkillIds.Count != 0)
        {
            var validSkills = await _context.Skills
                .Where(s => command.SkillIds.Contains(s.Id))
                .ToListAsync();

            foreach (var skill in validSkills)
            {
                stack.StackSkills.Add(new StackSkill
                {
                    IdStack = stack.Id,
                    IdSkill = skill.Id,
                    StackRole = StackRole.Backend // valor por defecto
                });
            }
        }

        var created = await _repository.CreateAsync(stack);
        return StackResult.Ok(MapToDto(created));
    }

    public async Task<StackResult> UpdateStackAsync(UpdateStackCommand command)
    {
        var existing = await _repository.GetByIdAsync(command.Id);
        if (existing is null)
            return StackResult.Fail($"No stack found with ID {command.Id}.");

        if (await _repository.ExistsBySummaryAsync(command.Summary, excludeId: command.Id))
            return StackResult.Fail($"Ya existe otro stack con el resumen '{command.Summary}'.");

        existing.Summary = command.Summary;
        existing.Category = command.Category;
        existing.IsActive = command.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        // Reemplazar skills asociadas
        var existingStackSkills = await _context.StackSkills
            .Where(ss => ss.IdStack == command.Id)
            .ToListAsync();
        _context.StackSkills.RemoveRange(existingStackSkills);

        if (command.SkillIds.Count != 0)
        {
            var validSkills = await _context.Skills
                .Where(s => command.SkillIds.Contains(s.Id))
                .ToListAsync();

            foreach (var skill in validSkills)
            {
                _context.StackSkills.Add(new StackSkill
                {
                    IdStack = command.Id,
                    IdSkill = skill.Id,
                    StackRole = StackRole.Backend
                });
            }
        }

        var updated = await _repository.UpdateAsync(existing);
        return StackResult.Ok(MapToDto(updated!));
    }

    public async Task<StackResult> SoftDeleteStackAsync(int id)
    {
        var stack = await _repository.SoftDeleteAsync(id);
        if (stack is null)
            return StackResult.Fail($"No stack found with ID {id}.");

        return StackResult.Ok(MapToDto(stack));
    }

    public async Task<StackResult> HardDeleteStackAsync(int id)
    {
        var stack = await _repository.GetByIdAsync(id);
        if (stack is null)
            return StackResult.Fail($"No stack found with ID {id}.");

        await _repository.HardDeleteAsync(id);
        return StackResult.Ok(MapToDto(stack));
    }

    // ---------- Mapeo interno ----------

    private StackAppDto MapToDto(Stack stack)
    {
        return new StackAppDto
        {
            Id = stack.Id,
            Summary = stack.Summary,
            Category = stack.Category,
            IsActive = stack.IsActive,
            CreatedAt = stack.CreatedAt,
            UpdatedAt = stack.UpdatedAt,
            Skills = stack.StackSkills?
                .Where(ss => ss.Skill != null)
                .Select(ss => new SkillRefAppDto
                {
                    Id = ss.Skill.Id,
                    Name = ss.Skill.Name,
                    ImageUrl = !string.IsNullOrEmpty(ss.Skill.ImageUrl)
                        ? _fileStorage.GetPublicUrl(ss.Skill.ImageUrl)
                        : null
                })
                .ToList() ?? new()
        };
    }
}
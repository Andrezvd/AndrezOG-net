namespace AndrezOG.Application;

using AndrezOG.Application.Commands.Skill;
using AndrezOG.Application.Dto;
using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Skills;
using AndrezOG.Shared.StorageService;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _repository;
    private readonly IFileStorageService _fileStorage;

    public SkillService(ISkillRepository skillRepository, IFileStorageService fileStorage)
    {
        _repository = skillRepository;
        _fileStorage = fileStorage;
    }

    // ---------- Lectura ----------

    public async Task<SkillResult> GetSkillByIdAsync(int id)
    {
        var skill = await _repository.GetByIdAsync(id);
        if (skill is null)
            return SkillResult.Fail($"No skill found with ID {id}.");

        return SkillResult.Ok(MapToDto(skill));
    }

    public async Task<SkillResult> GetAllSkillsAsync()
    {
        var skills = await _repository.ListAllAsync();
        return SkillResult.OkList(skills.Select(MapToDto).ToList());
    }

    public async Task<SkillResult> GetActiveSkillsAsync()
    {
        var skills = await _repository.ListActiveAsync();
        return SkillResult.OkList(skills.Select(MapToDto).ToList());
    }

    public async Task<SkillResult> GetSkillImageByIdAsync(int id)
    {
        var skill = await _repository.GetSkillImageByIdAsync(id);
        if (skill is null)
            return SkillResult.Fail($"No skill found with ID {id}.");

        return SkillResult.OkImage(skill.ImageUrl ?? string.Empty);
    }

    // ---------- Escritura ----------

    public async Task<SkillResult> CreateSkillAsync(CreateSkillCommand command)
    {
        // Idempotencia: evitar duplicados
        if (await _repository.ExistsByNameAsync(command.Name))
            return SkillResult.Fail($"Ya existe una skill con el nombre '{command.Name}'.");

        string? imageUrl = null;

        if (command.ImageFile is not null)
        {
            try
            {
                imageUrl = await _fileStorage.SaveFileAsync(command.ImageFile, "skills");
            }
            catch (Exception ex)
            {
                return SkillResult.Fail($"Error al guardar la imagen: {ex.Message}");
            }
        }

        var skill = new Skill
        {
            Name = command.Name,
            SkillType = command.SkillType,
            Description = command.Description,
            IsActive = command.IsActive,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(skill);
        return SkillResult.Ok(MapToDto(created));
    }

    public async Task<SkillResult> UpdateSkillAsync(UpdateSkillCommand command)
    {
        var existing = await _repository.GetByIdAsync(command.Id);
        if (existing is null)
            return SkillResult.Fail($"No skill found with ID {command.Id}.");

        // Idempotencia: evitar duplicados (excluyendo el registro actual)
        if (await _repository.ExistsByNameAsync(command.Name, excludeId: command.Id))
            return SkillResult.Fail($"Ya existe otra skill con el nombre '{command.Name}'.");

        string? imageUrl = existing.ImageUrl;

        // Eliminar imagen si se solicito
        if (command.RemoveImage && !string.IsNullOrWhiteSpace(existing.ImageUrl))
        {
            await _fileStorage.DeleteFileAsync(existing.ImageUrl);
            imageUrl = null;
        }

        // Reemplazar imagen si se envio nueva
        if (command.ImageFile is not null)
        {
            if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
                await _fileStorage.DeleteFileAsync(existing.ImageUrl);

            try
            {
                imageUrl = await _fileStorage.SaveFileAsync(command.ImageFile, "skills");
            }
            catch (Exception ex)
            {
                return SkillResult.Fail($"Error al guardar la imagen: {ex.Message}");
            }
        }

        existing.Name = command.Name;
        existing.SkillType = command.SkillType;
        existing.Description = command.Description;
        existing.IsActive = command.IsActive;
        existing.ImageUrl = imageUrl;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return SkillResult.Ok(MapToDto(updated!));
    }

    public async Task<SkillResult> SoftDeleteSkillAsync(int id)
    {
        var skill = await _repository.SoftDeleteAsync(id);
        if (skill is null)
            return SkillResult.Fail($"No skill found with ID {id}.");

        return SkillResult.Ok(MapToDto(skill));
    }

    public async Task<SkillResult> HardDeleteSkillAsync(int id)
    {
        var skill = await _repository.GetByIdAsync(id);
        if (skill is null)
            return SkillResult.Fail($"No skill found with ID {id}.");

        // Eliminar archivo de imagen
        if (!string.IsNullOrWhiteSpace(skill.ImageUrl))
            await _fileStorage.DeleteFileAsync(skill.ImageUrl);

        await _repository.HardDeleteAsync(id);
        return SkillResult.Ok(MapToDto(skill));
    }

    // ---------- Mapeo interno ----------

    private static SkillAppDto MapToDto(Skill skill) =>
        new()
        {
            Id = skill.Id,
            Name = skill.Name,
            SkillType = skill.SkillType,
            Description = skill.Description,
            IsActive = skill.IsActive,
            ImageUrl = skill.ImageUrl
        };
}

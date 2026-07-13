namespace AndrezOG.Application;

using AndrezOG.Application.Commands.Project;
using AndrezOG.Application.Dto.Project;
using AndrezOG.Application.Dto.Stack;
using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.StackProject;
using AndrezOG.Infrastructure.ContextDb;
using AndrezOG.Shared.StorageService;
using Microsoft.EntityFrameworkCore;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly AppDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public ProjectService(IProjectRepository repository, AppDbContext context, IFileStorageService fileStorage)
    {
        _repository = repository;
        _context = context;
        _fileStorage = fileStorage;
    }

    // ---------- Lectura ----------

    public async Task<ProjectResult> GetProjectByIdAsync(int id)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project is null)
            return ProjectResult.Fail($"No project found with ID {id}.");

        return ProjectResult.Ok(MapToDto(project));
    }

    public async Task<ProjectResult> GetAllProjectsAsync()
    {
        var projects = await _repository.ListAllAsync();
        return ProjectResult.OkList(projects.Select(MapToDto).ToList());
    }

    public async Task<ProjectResult> GetActiveProjectsAsync()
    {
        var projects = await _repository.ListActiveAsync();
        return ProjectResult.OkList(projects.Select(MapToDto).ToList());
    }

    // ---------- Escritura ----------

    public async Task<ProjectResult> CreateProjectAsync(CreateProjectCommand command)
    {
        if (await _repository.ExistsByTitleAsync(command.Title))
            return ProjectResult.Fail($"Ya existe un proyecto con el título '{command.Title}'.");

        string? imageUrl = null;

        if (command.ImageFile is not null)
        {
            try
            {
                imageUrl = await _fileStorage.SaveFileAsync(command.ImageFile, "projects");
            }
            catch (Exception ex)
            {
                return ProjectResult.Fail($"Error al guardar la imagen: {ex.Message}");
            }
        }

        var project = new Project
        {
            Title = command.Title,
            Description = command.Description,
            StartDate = DateTime.SpecifyKind(command.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(command.EndDate, DateTimeKind.Utc),
            IsActive = command.IsActive,
            RepositoryUrl = command.RepositoryUrl,
            Type = command.Type,
            ImageUrl = imageUrl
        };

        // Guardar primero para obtener el ID generado
        var created = await _repository.CreateAsync(project);

        // Asociar stacks (ahora project.Id ya existe)
        if (command.StackIds.Count != 0)
        {
            var validStacks = await _context.Stacks
                .Where(s => command.StackIds.Contains(s.Id))
                .ToListAsync();

            foreach (var stack in validStacks)
            {
                _context.StackProjects.Add(new StackProject
                {
                    IdProject = created.Id,
                    IdStack = stack.Id
                });
            }
            await _context.SaveChangesAsync();
        }

        // Recargar con includes para el DTO
        var fullProject = await _repository.GetByIdAsync(created.Id);
        return ProjectResult.Ok(MapToDto(fullProject ?? created));
    }

    public async Task<ProjectResult> UpdateProjectAsync(UpdateProjectCommand command)
    {
        var existing = await _repository.GetByIdAsync(command.Id);
        if (existing is null)
            return ProjectResult.Fail($"No project found with ID {command.Id}.");

        if (await _repository.ExistsByTitleAsync(command.Title, excludeId: command.Id))
            return ProjectResult.Fail($"Ya existe otro proyecto con el título '{command.Title}'.");

        string? imageUrl = existing.ImageUrl;

        // Eliminar imagen si se solicitó
        if (command.RemoveImage && !string.IsNullOrWhiteSpace(existing.ImageUrl))
        {
            await _fileStorage.DeleteFileAsync(existing.ImageUrl);
            imageUrl = null;
        }

        // Reemplazar imagen si se envió nueva
        if (command.ImageFile is not null)
        {
            if (!string.IsNullOrWhiteSpace(existing.ImageUrl))
                await _fileStorage.DeleteFileAsync(existing.ImageUrl);

            try
            {
                imageUrl = await _fileStorage.SaveFileAsync(command.ImageFile, "projects");
            }
            catch (Exception ex)
            {
                return ProjectResult.Fail($"Error al guardar la imagen: {ex.Message}");
            }
        }

        existing.Title = command.Title;
        existing.Description = command.Description;
        existing.StartDate = DateTime.SpecifyKind(command.StartDate, DateTimeKind.Utc);
        existing.EndDate = DateTime.SpecifyKind(command.EndDate, DateTimeKind.Utc);
        existing.IsActive = command.IsActive;
        existing.RepositoryUrl = command.RepositoryUrl;
        existing.Type = command.Type;
        existing.ImageUrl = imageUrl;

        // Reemplazar stacks asociados
        var existingStackProjects = await _context.StackProjects
            .Where(sp => sp.IdProject == command.Id)
            .ToListAsync();
        _context.StackProjects.RemoveRange(existingStackProjects);

        if (command.StackIds.Count != 0)
        {
            var validStacks = await _context.Stacks
                .Where(s => command.StackIds.Contains(s.Id))
                .ToListAsync();

            foreach (var stack in validStacks)
            {
                _context.StackProjects.Add(new StackProject
                {
                    IdProject = command.Id,
                    IdStack = stack.Id
                });
            }
        }

        var updated = await _repository.UpdateAsync(existing);
        return ProjectResult.Ok(MapToDto(updated!));
    }

    public async Task<ProjectResult> SoftDeleteProjectAsync(int id)
    {
        var project = await _repository.SoftDeleteAsync(id);
        if (project is null)
            return ProjectResult.Fail($"No project found with ID {id}.");

        return ProjectResult.Ok(MapToDto(project));
    }

    public async Task<ProjectResult> HardDeleteProjectAsync(int id)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project is null)
            return ProjectResult.Fail($"No project found with ID {id}.");

        if (!string.IsNullOrWhiteSpace(project.ImageUrl))
            await _fileStorage.DeleteFileAsync(project.ImageUrl);

        await _repository.HardDeleteAsync(id);
        return ProjectResult.Ok(MapToDto(project));
    }

    // ---------- Mapeo interno ----------

    private ProjectAppDto MapToDto(Project project)
    {
        var relativeUrl = project.ImageUrl;
        var publicUrl = !string.IsNullOrEmpty(relativeUrl)
            ? _fileStorage.GetPublicUrl(relativeUrl)
            : null;

        return new ProjectAppDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            RepositoryUrl = project.RepositoryUrl,
            Type = project.Type.ToString(),
            ImageUrl = publicUrl,
            Stacks = project.StackProjects?
                .Where(sp => sp.Stack != null)
                .Select(sp => new StackSummaryAppDto
                {
                    Id = sp.Stack.Id,
                    Summary = sp.Stack.Summary,
                    Category = sp.Stack.Category,
                    Skills = sp.Stack.StackSkills?
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
                })
                .ToList() ?? new()
        };
    }
}
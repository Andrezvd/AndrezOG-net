namespace AndrezOG.Infrastructure.Repository;

using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.StackProject;
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    // ---------- Lectura ----------

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.StackProjects)
                .ThenInclude(sp => sp.Stack)
                    .ThenInclude(s => s.StackSkills)
                        .ThenInclude(ss => ss.Skill)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Project>> ListAllAsync()
    {
        return await _context.Projects
            .Include(p => p.StackProjects)
                .ThenInclude(sp => sp.Stack)
                    .ThenInclude(s => s.StackSkills)
                        .ThenInclude(ss => ss.Skill)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();
    }

    public async Task<List<Project>> ListActiveAsync()
    {
        return await _context.Projects
            .Where(p => p.IsActive == true)
            .Include(p => p.StackProjects)
                .ThenInclude(sp => sp.Stack)
                    .ThenInclude(s => s.StackSkills)
                        .ThenInclude(ss => ss.Skill)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();
    }

    // ---------- Escritura ----------

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> UpdateAsync(Project project)
    {
        _context.Entry(project).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> SoftDeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null) return null;

        project.IsActive = false;
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> HardDeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null) return null;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return project;
    }

    // ---------- Validación ----------

    public async Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
    {
        var query = _context.Projects.Where(p => p.Title == title);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
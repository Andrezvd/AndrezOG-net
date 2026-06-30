namespace AndrezOG.Infrastructure.Repository;

using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Skills;
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;

public class SkillRepository : ISkillRepository
{
    private readonly AppDbContext _context;

    public SkillRepository(AppDbContext context)
    {
        _context = context;
    }

    // ---------- Lectura ----------

    public async Task<Skill?> GetByIdAsync(int id)
    {
        return await _context.Skills.FindAsync(id);
    }

    public async Task<List<Skill>> ListAllAsync()
    {
        return await _context.Skills
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<Skill>> ListActiveAsync()
    {
        return await _context.Skills
            .Where(s => s.IsActive == true)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Skill?> GetSkillImageByIdAsync(int id)
    {
        return await _context.Skills
            .Where(s => s.Id == id)
            .Select(s => new Skill
            {
                Id = s.Id,
                Name = s.Name,
                ImageUrl = s.ImageUrl,
                SkillType = s.SkillType,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync();
    }

    // ---------- Escritura ----------

    public async Task<Skill> CreateAsync(Skill skill)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task<Skill?> UpdateAsync(Skill skill)
    {
        _context.Entry(skill).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task<Skill?> SoftDeleteAsync(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill is null) return null;

        skill.IsActive = false;
        skill.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task<Skill?> HardDeleteAsync(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill is null) return null;

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        return skill;
    }

    // ---------- Validacion ----------

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Skills.Where(s => s.Name == name);
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }
}

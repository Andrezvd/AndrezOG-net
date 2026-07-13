namespace AndrezOG.Infrastructure.Repository;

using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.StackProject;
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;

public class StackRepository : IStackRepository
{
    private readonly AppDbContext _context;

    public StackRepository(AppDbContext context)
    {
        _context = context;
    }

    // ---------- Lectura ----------

    public async Task<Stack?> GetByIdAsync(int id)
    {
        return await _context.Stacks
            .Include(s => s.StackSkills)
                .ThenInclude(ss => ss.Skill)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Stack>> ListAllAsync()
    {
        return await _context.Stacks
            .Include(s => s.StackSkills)
                .ThenInclude(ss => ss.Skill)
            .OrderBy(s => s.Summary)
            .ToListAsync();
    }

    public async Task<List<Stack>> ListActiveAsync()
    {
        return await _context.Stacks
            .Where(s => s.IsActive == true)
            .Include(s => s.StackSkills)
                .ThenInclude(ss => ss.Skill)
            .OrderBy(s => s.Summary)
            .ToListAsync();
    }

    // ---------- Escritura ----------

    public async Task<Stack> CreateAsync(Stack stack)
    {
        _context.Stacks.Add(stack);
        await _context.SaveChangesAsync();
        return stack;
    }

    public async Task<Stack?> UpdateAsync(Stack stack)
    {
        _context.Entry(stack).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return stack;
    }

    public async Task<Stack?> SoftDeleteAsync(int id)
    {
        var stack = await _context.Stacks.FindAsync(id);
        if (stack is null) return null;

        stack.IsActive = false;
        stack.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return stack;
    }

    public async Task<Stack?> HardDeleteAsync(int id)
    {
        var stack = await _context.Stacks.FindAsync(id);
        if (stack is null) return null;

        _context.Stacks.Remove(stack);
        await _context.SaveChangesAsync();
        return stack;
    }

    // ---------- Validación ----------

    public async Task<bool> ExistsBySummaryAsync(string summary, int? excludeId = null)
    {
        var query = _context.Stacks.Where(s => s.Summary == summary);
        if (excludeId.HasValue)
            query = query.Where(s => s.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
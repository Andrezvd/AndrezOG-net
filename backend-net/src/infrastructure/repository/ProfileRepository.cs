namespace AndrezOG.Infrastructure.Repository;

using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;
using AndrezOG.Infrastructure.ContextDb;

public class ProfileRepository : IProfileRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Profile profile)
    {
        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();
    }
}

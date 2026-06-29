namespace AndrezOG.Infrastructure.Repository;

using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;
using AndrezOG.Infrastructure.ContextDb;
using Microsoft.EntityFrameworkCore;

public class ProfileRepository : IProfileRepository
{
    private readonly AppDbContext _context;

    public ProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Profile?> GetByUserIdAsync(int userId)
    {
        return await _context.Profiles.FirstOrDefaultAsync(profile => profile.IdUser == userId);
    }

    public async Task CreateAsync(Profile profile)
    {
        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Profile profile)
    {
        _context.Profiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    public async Task<Profile?> GetMyProfileAsync()
    {
        string email = "andreziwis@gmail.com";
        var profile = await _context.Profiles.FirstOrDefaultAsync(profile => profile.Email == email);
        return profile;
    }

    public async Task<Profile?> GetPublicProfileAsync()
    {
        return await _context.Profiles.FirstOrDefaultAsync(p => p.IdUser == 5);
    }
}
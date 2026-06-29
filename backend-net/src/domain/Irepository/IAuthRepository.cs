namespace AndrezOG.Domain.Irepository;

using AndrezOG.Domain.Model.Tenant;
public interface IAuthRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByGoogleIdAsync(string googleId);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> EmailExistsAsync(string email);
    Task<User> CreateAsync(User user);
    User Add(User user);
    Task UpdateAsync(User user);
}

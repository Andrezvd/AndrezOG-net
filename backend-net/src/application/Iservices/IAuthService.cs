namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Result;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterCommand command);
    Task<AuthResult> LoginAsync(LoginCommand command);
    Task<AuthResult> GoogleLoginAsync(GoogleLoginCommand command);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenCommand command);
    Task LogoutAsync(int userId);
}

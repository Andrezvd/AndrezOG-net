namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Result;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterCommand command);
    Task<AuthResult> RegisterWithProfileAsync(RegisterCommand userCommand, CreateDefaultProfileCommand profileCommand);
    Task<AuthResult> LoginAsync(LoginCommand command);
    Task<AuthResult> ExternalLoginAsync(ExternalLoginCommand command);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(int userId);
    string GenerateRefreshToken();
    Task<AuthResult> VerifyEmailAsync(string token);
}

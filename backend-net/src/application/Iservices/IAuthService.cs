namespace AndrezOG.Application.Iservices;

using AndrezOG.Application.Result;
using AndrezOG.Api.Rest.Dto.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
}
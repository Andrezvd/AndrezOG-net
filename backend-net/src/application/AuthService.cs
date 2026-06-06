namespace AndrezOG.Application;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<AuthResult> RegisterAsync(RegisterCommand command)
    {
        if (await _repository.EmailExistsAsync(command.Email))
        {
            return new AuthResult
            {
                Success = false,
                Message = "El email ya esta registrado"
            };
        }

        var user = new User
        {
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(user);

        var token = GenerateJwtToken(user);
        return new AuthResult
        {
            Success = true,
            Message = "Usuario registrado exitosamente",
            UserId = user.Id,
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResult> LoginAsync(LoginCommand command)
    {
        var user = await _repository.GetByEmailAsync(command.Email);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
        {
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        var token = GenerateJwtToken(user);
        return new AuthResult
        {
            Success = true,
            Message = "Usuario autenticado exitosamente",
            UserId = user.Id,
            Token = token,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "ClaveDeDesarrolloLocal-SoloParaFallback-2025!";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AndrezOG",
            audience: _configuration["Jwt:Audience"] ?? "AndrezOG-Client",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

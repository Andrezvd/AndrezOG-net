namespace AndrezOG.Application;

using AndrezOG.Application.Commands;
using AndrezOG.Application.Iservices;
using AndrezOG.Application.Result;
using AndrezOG.Domain.Irepository;
using AndrezOG.Domain.Model.Tenant;

using AndrezOG.Infrastructure.ContextDb;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IProfileRepository _profileRepository;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthService(
        IAuthRepository repository,
        IProfileRepository profileRepository,
        IConfiguration configuration,
        AppDbContext context)
    {
        _repository = repository;
        _profileRepository = profileRepository;
        _configuration = configuration;
        _context = context;
    }

    // ================================================================
    // REGISTRO MANUAL (email + password)
    // ================================================================

    public async Task<AuthResult> RegisterAsync(RegisterCommand command)
    {
        if (!IsPasswordStrong(command.Password))
        {
            return new AuthResult
            {
                Success = false,
                Message = "La contraseña debe tener al menos 8 caracteres, 1 mayúscula, 1 número y 1 carácter especial."
            };
        }

        if (await _repository.EmailExistsAsync(command.Email))
        {
            return new AuthResult
            {
                Success = false,
                Message = "El email ya está registrado"
            };
        }

        var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>();
        var role = adminEmails.Contains(command.Email, StringComparer.OrdinalIgnoreCase)
            ? UserRole.Admin
            : UserRole.Client;

        var user = new User
        {
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            Role = role,
            Provider = AuthProvider.Local,
            EmailVerified = false,
            EmailVerificationToken = GenerateVerificationToken(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(user);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, refreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Usuario registrado exitosamente",
            UserId = user.Id,
            Email = user.Email,
            Name = string.Empty,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// Registro de usuario + creación de perfil en una transacción atómica.
    /// Si cualquiera de las dos falla, se hace rollback automático.
    /// </summary>
    public async Task<AuthResult> RegisterWithProfileAsync(RegisterCommand userCommand, CreateDefaultProfileCommand profileCommand)
    {
        if (!IsPasswordStrong(userCommand.Password))
        {
            return new AuthResult
            {
                Success = false,
                Message = "La contraseña debe tener al menos 8 caracteres, 1 mayúscula, 1 número y 1 carácter especial."
            };
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (await _repository.EmailExistsAsync(userCommand.Email))
            {
                await transaction.RollbackAsync();
                return new AuthResult
                {
                    Success = false,
                    Message = "El email ya está registrado"
                };
            }

            var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>();
            var role = adminEmails.Contains(userCommand.Email, StringComparer.OrdinalIgnoreCase)
                ? UserRole.Admin
                : UserRole.Client;

            // 1. Crear usuario
            var user = new User
            {
                Email = userCommand.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCommand.Password),
                Role = role,
                Provider = AuthProvider.Local,
                EmailVerified = false,
                EmailVerificationToken = GenerateVerificationToken(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(user);

            // 2. Crear perfil por defecto
            var profile = new Profile
            {
                IdUser = user.Id,
                Name = profileCommand.Name,
                LastName = profileCommand.LastName,
                PhoneNumber = profileCommand.PhoneNumber,
                Country = profileCommand.Country,
                Title = string.Empty,
                Education = string.Empty,
                EducationStartYear = string.Empty,
                EducationEndYear = string.Empty,
                Email = profileCommand.Email,
                Available = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _profileRepository.CreateAsync(profile);

            await transaction.CommitAsync();

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user, refreshToken);

            return new AuthResult
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                UserId = user.Id,
                Email = user.Email,
                Name = profileCommand.Name,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ================================================================
    // LOGIN MANUAL (email + password)
    // ================================================================

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

        // Verificar lockout
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remaining = user.LockoutEnd.Value - DateTime.UtcNow;
            return new AuthResult
            {
                Success = false,
                Message = $"Cuenta bloqueada temporalmente. Intenta de nuevo en {remaining.Minutes} minuto(s)."
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
        {
            user.FailedAttempts++;

            if (user.FailedAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                user.FailedAttempts = 0;
                await _repository.UpdateAsync(user);
                return new AuthResult
                {
                    Success = false,
                    Message = "Cuenta bloqueada por 15 minutos debido a múltiples intentos fallidos."
                };
            }

            await _repository.UpdateAsync(user);
            return new AuthResult
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }

        // Login exitoso
        user.FailedAttempts = 0;
        user.LockoutEnd = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, refreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Usuario autenticado exitosamente",
            UserId = user.Id,
            Email = user.Email,
            Name = string.Empty,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refreshToken
        };
    }

    // ================================================================
    // LOGIN EXTERNO (Google, GitHub, etc.)
    // ================================================================

    public async Task<AuthResult> ExternalLoginAsync(ExternalLoginCommand command)
    {
        if (!Enum.TryParse<AuthProvider>(command.Provider, ignoreCase: true, out var provider))
        {
            return new AuthResult
            {
                Success = false,
                Message = $"Provider no soportado: {command.Provider}"
            };
        }

        return provider switch
        {
            AuthProvider.Google => await GoogleLoginAsync(command.IdToken),
            _ => new AuthResult { Success = false, Message = $"Provider no soportado: {command.Provider}" }
        };
    }

    private async Task<AuthResult> GoogleLoginAsync(string idToken)
    {
        // Validar ID Token de Google (criptográficamente, sin HTTP a Google)
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var googleClientId = _configuration["Google:ClientId"]
                ?? throw new InvalidOperationException("Google:ClientId no configurado");

            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            };

            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch
        {
            return new AuthResult
            {
                Success = false,
                Message = "Token de Google inválido o expirado."
            };
        }

        if (!payload.EmailVerified)
        {
            return new AuthResult
            {
                Success = false,
                Message = "El email de Google no está verificado."
            };
        }

        var googleId = payload.Subject;
        var email = payload.Email;

        // Buscar usuario por GoogleId primero (inmutable), luego por email
        var user = await _repository.GetByExternalIdAsync(googleId)
                   ?? await _repository.GetByEmailAsync(email);

        if (user != null)
        {
            // Vincular GoogleId si no lo tiene
            if (string.IsNullOrEmpty(user.ExternalId))
            {
                user.ExternalId = googleId;
                user.Provider = AuthProvider.Google;
                user.EmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(user);
            }
        }
        else
        {
            // Nuevo usuario
            var adminEmails = _configuration.GetSection("AdminEmails").Get<string[]>() ?? Array.Empty<string>();
            var role = adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase)
                ? UserRole.Admin
                : UserRole.Client;

            user = new User
            {
                Email = email,
                ExternalId = googleId,
                Provider = AuthProvider.Google,
                PasswordHash = string.Empty,
                Role = role,
                EmailVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(user);
        }

        var jwtToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, refreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Autenticado con Google exitosamente",
            UserId = user.Id,
            Email = user.Email,
            Name = payload.GivenName,
            LastName = payload.FamilyName,
            Role = user.Role.ToString(),
            Token = jwtToken,
            RefreshToken = refreshToken
        };
    }

    // ================================================================
    // REFRESH TOKEN
    // ================================================================

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenHash = HashToken(refreshToken);
        var user = await _repository.GetByRefreshTokenAsync(refreshTokenHash);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Refresh token inválido o expirado."
            };
        }

        var newJwt = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        await SaveRefreshToken(user, newRefreshToken);

        return new AuthResult
        {
            Success = true,
            Message = "Token renovado exitosamente",
            UserId = user.Id,
            Token = newJwt,
            RefreshToken = newRefreshToken,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    // ================================================================
    // LOGOUT
    // ================================================================

    public async Task LogoutAsync(int userId)
    {
        var user = await _repository.GetByIdAsync(userId);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpires = null;
            await _repository.UpdateAsync(user);
        }
    }

    // ================================================================
    // EMAIL VERIFICATION
    // ================================================================

    public async Task<AuthResult> VerifyEmailAsync(string token)
    {
        var user = await _repository.GetByEmailVerificationTokenAsync(token);
        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                Message = "Token de verificación inválido o expirado."
            };
        }

        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(user);

        return new AuthResult
        {
            Success = true,
            Message = "Email verificado exitosamente.",
            Email = user.Email
        };
    }

    // ================================================================
    // MÉTODOS PRIVADOS
    // ================================================================

    public string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("Jwt:Key no configurado.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
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

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private async Task SaveRefreshToken(User user, string refreshToken)
    {
        user.RefreshToken = HashToken(refreshToken);
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
        await _repository.UpdateAsync(user);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    private static string GenerateVerificationToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }
}
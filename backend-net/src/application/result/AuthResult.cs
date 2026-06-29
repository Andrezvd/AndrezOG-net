namespace AndrezOG.Application.Result;

public class AuthResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int? UserId { get; set; }
    public string? Token { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
}

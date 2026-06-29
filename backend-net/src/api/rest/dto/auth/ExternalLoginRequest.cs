namespace AndrezOG.Api.Rest.Dto.Auth;

public class ExternalLoginRequest
{
    public string Provider { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
}
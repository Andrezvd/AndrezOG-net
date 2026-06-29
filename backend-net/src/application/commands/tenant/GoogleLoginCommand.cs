namespace AndrezOG.Application.Commands;

public record GoogleLoginCommand(string Code, string RedirectUri);
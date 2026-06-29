namespace AndrezOG.Application.Commands;

public record ExternalLoginCommand(string Provider, string IdToken);
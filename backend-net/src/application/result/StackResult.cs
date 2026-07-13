namespace AndrezOG.Application.Result;

using AndrezOG.Application.Dto.Stack;

public class StackResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public StackAppDto? Stack { get; set; }
    public List<StackAppDto>? Stacks { get; set; }

    public static StackResult Ok(StackAppDto stack) =>
        new() { Success = true, Stack = stack };

    public static StackResult OkList(List<StackAppDto> stacks) =>
        new() { Success = true, Stacks = stacks };

    public static StackResult Fail(string message) =>
        new() { Success = false, ErrorMessage = message };
}
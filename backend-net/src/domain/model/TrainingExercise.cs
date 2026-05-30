namespace AndrezOG.Domain.Model;

public class TrainingExercise
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public string InitialCodeCsharp { get; set; } = string.Empty;
    public string InitialCodePython { get; set; } = string.Empty;
    public string InitialCodeJava { get; set; } = string.Empty;
    public string InitialCodeTypeScript { get; set; } = string.Empty;
    public string TestCases { get; set; } = string.Empty;
    public string? Solution { get; set; } = string.Empty;
    public string InputExample { get; set; } = string.Empty;
    public string OutputExample { get; set; } = string.Empty;
    public string? ImagesExample { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int Order { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
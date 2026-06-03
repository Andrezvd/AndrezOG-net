namespace AndrezOG.Domain.Model.Contact;

public class ContactMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;
    public ContactType Type { get; set; }
    public string? ServiceInterest { get; set; }
    public bool IsRead { get; set; }
    public bool NeedsAttention { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
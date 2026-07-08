namespace AndrezOG.Api.Rest.Dto.Profile;

using System.ComponentModel.DataAnnotations;

public class UpdateRequest
{
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string? Name { get; set; }

    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string? LastName { get; set; }

    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? PhoneNumber { get; set; }

    [StringLength(50, ErrorMessage = "El país no puede exceder 50 caracteres")]
    public string? Country { get; set; }

    [StringLength(50, ErrorMessage = "La ciudad no puede exceder 50 caracteres")]
    public string? City { get; set; }

    [StringLength(50, ErrorMessage = "El estado no puede exceder 50 caracteres")]
    public string? State { get; set; }

    [StringLength(10, ErrorMessage = "El código postal no puede exceder 10 caracteres")]
    public string? ZipCode { get; set; }

    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string? Title { get; set; }

    [StringLength(500, ErrorMessage = "El resumen no puede exceder 500 caracteres")]
    public string? Summary { get; set; }

    public bool? Available { get; set; }

    public string? AvailableText { get; set; }

    [StringLength(200, ErrorMessage = "La educación no puede exceder 200 caracteres")]
    public string? Education { get; set; }

    public string? EducationStartYear { get; set; }

    public string? EducationEndYear { get; set; }

    [StringLength(500, ErrorMessage = "La URL de foto no puede exceder 500 caracteres")]
    public string? PhotoUrl { get; set; }

    [StringLength(500, ErrorMessage = "La URL de video no puede exceder 500 caracteres")]
    public string? VideoUrl { get; set; }

    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "La URL de LinkedIn no puede exceder 500 caracteres")]
    public string? LinkedInUrl { get; set; }

    [StringLength(500, ErrorMessage = "La URL de GitHub no puede exceder 500 caracteres")]
    public string? GitHubUrl { get; set; }
}

namespace AndrezOG.Shared.StorageService;

using Microsoft.AspNetCore.Http;

public interface IFileStorageService
{
    /// <summary>
    /// Guarda un archivo y retorna la ruta relativa (ej: "profiles/abc123.png").
    /// </summary>
    Task<string> SaveFileAsync(IFormFile file, string subfolder);

    /// <summary>
    /// Construye la URL completa accesible públicamente a partir de una ruta relativa.
    /// En local: "/uploads/profiles/abc.jpg"
    /// En GCS: "https://storage.googleapis.com/bucket/profiles/abc.jpg"
    /// </summary>
    string GetPublicUrl(string relativePath);

    /// <summary>
    /// Elimina un archivo. No debe lanzar excepción si el archivo no existe.
    /// </summary>
    Task DeleteFileAsync(string? fileUrl);
}
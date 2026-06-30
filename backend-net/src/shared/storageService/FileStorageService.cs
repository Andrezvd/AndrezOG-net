namespace AndrezOG.Shared.StorageService;

using Microsoft.AspNetCore.Http;

public class FileStorageService
{
    private readonly string _basePath;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".ico"
    };
    public FileStorageService(string basePath)
    {
        _basePath = basePath;
    }
    
    /// <summary>
    /// Guarda un IFormFile en disco y retorna la ruta relativa pública.
    /// </summary>
    /// <param name="file">Archivo recibido del frontend.</param>
    /// <param name="subfolder">Subcarpeta dentro de uploads/ (ej: "skills").</param>
    /// <returns>Ruta relativa: "/uploads/skills/abc123.png"</returns>
    
    public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("El archivo no puede estar vacío.", nameof(file));
        }
        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                $"Extensión '{extension}' no permitida. Solo se permiten: {string.Join(", ", AllowedExtensions)}"
            );
        }

        if (file.Length > 5 * 1024 * 1024) // 5MB
        {
            throw new InvalidOperationException("El archivo excede el tamaño máximo permitido de 5MB.");
        }

        var uploadDir = Path.Combine(_basePath, "uploads", subfolder);
        Directory.CreateDirectory(uploadDir);

        var uniqueName = $"{Guid.NewGuid()}{extension}";

        var fullPath = Path.Combine(uploadDir, uniqueName);
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"{subfolder}/{uniqueName}";
    }

        /// <summary>
        /// Elimina un archivo del disco.
        /// </summary>
        /// <param name="relativePath">Ruta relativa del archivo a eliminar.</param>
        
    public void DeleteFile(string? fileName, string subfolder)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        // Extraer solo el nombre del archivo por si viene con path completo (compatibilidad hacia atrás)
        var name = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_basePath, "uploads", subfolder, name);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
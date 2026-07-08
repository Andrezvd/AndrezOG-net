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

        if (file.Length > 5 * 1024 * 1024) // 5MB
        {
            throw new InvalidOperationException("El archivo excede el tamaño máximo permitido de 5MB.");
        }

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                $"Extensión '{extension}' no permitida. Solo se permiten: {string.Join(", ", AllowedExtensions)}"
            );
        }

        // Validación de MIME type real mediante los primeros bytes del archivo
        var allowedMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp",
            "image/svg+xml",
            "image/vnd.microsoft.icon",
            "image/x-icon"
        };

        // Usar un MemoryStream para leer los primeros bytes y determinar MIME
        await using var fileStream = file.OpenReadStream();
        var headerBytes = new byte[8];
        var bytesRead = await fileStream.ReadAsync(headerBytes.AsMemory(0, 8));
        fileStream.Position = 0;

        // Detección de firmas mágicas
        var detectedType = DetectMimeType(headerBytes, bytesRead);
        if (string.IsNullOrEmpty(detectedType) || !allowedMimeTypes.Contains(detectedType))
        {
            throw new InvalidOperationException(
                "El tipo de archivo no es válido. Solo se permiten imágenes."
            );
        }

        var uploadDir = Path.Combine(_basePath, "uploads", subfolder);
        Directory.CreateDirectory(uploadDir);

        var uniqueName = $"{Guid.NewGuid()}{extension}";

        var fullPath = Path.Combine(uploadDir, uniqueName);
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(stream);
        return $"{subfolder}/{uniqueName}";
    }

    /// <summary>
    /// Detecta el tipo MIME basado en los primeros bytes (firmas mágicas).
    /// </summary>
    private static string? DetectMimeType(byte[] header, int bytesRead)
    {
        if (bytesRead < 4) return null;

        // JPEG: FF D8 FF
        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return "image/jpeg";

        // PNG: 89 50 4E 47
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
            return "image/png";

        // GIF: 47 49 46 38 (GIF8)
        if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38)
            return "image/gif";

        // BMP: 42 4D
        if (header[0] == 0x42 && header[1] == 0x4D)
            return "image/bmp";

        // SVG/XML: depende de contenido textual, no se puede detectar solo con header
        // ICO: 00 00 01 00
        if (bytesRead >= 4 && header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x01 && header[3] == 0x00)
            return "image/x-icon";

        // Para SVG e ICO confiamos en la extensión (ya validada arriba)
        return null;
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
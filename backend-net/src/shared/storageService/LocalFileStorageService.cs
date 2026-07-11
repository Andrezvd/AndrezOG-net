namespace AndrezOG.Shared.StorageService;

using Microsoft.AspNetCore.Http;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".ico"
    };

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _basePath = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("El archivo no puede estar vacío.", nameof(file));

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("El archivo excede el tamaño máximo permitido de 5MB.");

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                $"Extensión '{extension}' no permitida. Solo se permiten: {string.Join(", ", AllowedExtensions)}");

        // Validación de MIME type real mediante firmas mágicas
        await using var fileStream = file.OpenReadStream();
        var headerBytes = new byte[8];
        var bytesRead = await fileStream.ReadAsync(headerBytes.AsMemory(0, 8));
        fileStream.Position = 0;

        var detectedType = DetectMimeType(headerBytes, bytesRead);
        if (string.IsNullOrEmpty(detectedType) || !IsAllowedMimeType(detectedType))
            throw new InvalidOperationException("El tipo de archivo no es válido. Solo se permiten imágenes.");

        var uploadDir = Path.Combine(_basePath, "uploads", subfolder);
        Directory.CreateDirectory(uploadDir);

        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var relativePath = $"{subfolder}/{uniqueName}";
        var fullPath = Path.Combine(uploadDir, uniqueName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return relativePath;
    }

    public string GetPublicUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        return $"/uploads/{relativePath}";
    }

    public Task DeleteFileAsync(string? fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return Task.CompletedTask;

        var path = fileUrl.StartsWith("/uploads/")
            ? fileUrl["/uploads/".Length..]
            : fileUrl;

        var fullPath = Path.Combine(_basePath, "uploads", path.Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private static string? DetectMimeType(byte[] header, int bytesRead)
    {
        if (bytesRead < 4) return null;

        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return "image/jpeg";
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
            return "image/png";
        if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38)
            return "image/gif";
        if (header[0] == 0x42 && header[1] == 0x4D)
            return "image/bmp";
        if (bytesRead >= 4 && header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x01 && header[3] == 0x00)
            return "image/x-icon";

        return null;
    }

    private static bool IsAllowedMimeType(string mimeType)
    {
        return mimeType switch
        {
            "image/jpeg" => true,
            "image/png" => true,
            "image/gif" => true,
            "image/bmp" => true,
            "image/svg+xml" => true,
            "image/vnd.microsoft.icon" => true,
            "image/x-icon" => true,
            _ => false
        };
    }
}
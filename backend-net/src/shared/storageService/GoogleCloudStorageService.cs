namespace AndrezOG.Shared.StorageService;

using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class GoogleCloudStorageService : IFileStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;
    private readonly ILogger<GoogleCloudStorageService> _logger;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".ico"
    };

    public GoogleCloudStorageService(
        StorageClient storageClient,
        IOptions<StorageOptions> options,
        ILogger<GoogleCloudStorageService> logger)
    {
        _storageClient = storageClient;
        _bucketName = options.Value.BucketName;
        _logger = logger;
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

        await using var fileStream = file.OpenReadStream();
        var headerBytes = new byte[8];
        var bytesRead = await fileStream.ReadAsync(headerBytes.AsMemory(0, 8));
        fileStream.Position = 0;

        var detectedType = DetectMimeType(headerBytes, bytesRead);
        if (string.IsNullOrEmpty(detectedType) || !IsAllowedMimeType(detectedType))
            throw new InvalidOperationException("El tipo de archivo no es válido. Solo se permiten imágenes.");

        var uniqueName = $"{Guid.NewGuid()}{extension}";
        var blobName = $"{subfolder}/{uniqueName}";

        try
        {
            await _storageClient.UploadObjectAsync(
                _bucketName, blobName, detectedType, fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir archivo {BlobName} a GCS bucket {Bucket}",
                blobName, _bucketName);
            throw new InvalidOperationException("Error al guardar el archivo en el almacenamiento.");
        }

        return blobName;
    }

    public string GetPublicUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        return $"https://storage.googleapis.com/{_bucketName}/{relativePath}";
    }

    public async Task DeleteFileAsync(string? fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return;

        try
        {
            var blobName = ExtractBlobName(fileUrl);
            if (string.IsNullOrWhiteSpace(blobName))
                return;

            await _storageClient.DeleteObjectAsync(_bucketName, blobName);
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Intento de eliminar archivo inexistente en GCS: {FileUrl}", fileUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo {FileUrl} de GCS", fileUrl);
        }
    }

    private string? ExtractBlobName(string fileUrl)
    {
        if (Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
        {
            var path = uri.AbsolutePath.TrimStart('/');
            var bucketPrefix = $"{_bucketName}/";
            if (path.StartsWith(bucketPrefix))
                return path[bucketPrefix.Length..];
            return null;
        }
        return fileUrl;
    }

    private static string? DetectMimeType(byte[] header, int bytesRead)
    {
        if (bytesRead < 4) return null;
        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) return "image/jpeg";
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) return "image/png";
        if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) return "image/gif";
        if (header[0] == 0x42 && header[1] == 0x4D) return "image/bmp";
        if (bytesRead >= 4 && header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x01 && header[3] == 0x00) return "image/x-icon";
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
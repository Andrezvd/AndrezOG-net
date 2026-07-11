namespace AndrezOG.Shared.StorageService;

public class StorageOptions
{
    /// <summary>
    /// Proveedor de almacenamiento: "Local" para desarrollo, "GoogleCloud" para producción.
    /// </summary>
    public string Provider { get; set; } = "Local";

    /// <summary>
    /// Nombre del bucket GCS (solo usado cuando Provider=GoogleCloud).
    /// </summary>
    public string BucketName { get; set; } = "andrezog-uploads";
}
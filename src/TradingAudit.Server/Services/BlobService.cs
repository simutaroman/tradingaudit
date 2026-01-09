using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace TradingAudit.Server.Services;

public interface IBlobService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteAsync(string fileUrl);
}

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:BlobConnectionString"];
        var containerName = configuration["AzureStorage:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // На всяк випадок створюємо контейнер, якщо його немає
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
    {
        // Генеруємо унікальне ім'я, щоб не перезаписати існуючі файли
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var blobClient = _containerClient.GetBlobClient(uniqueName);

        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, blobUploadOptions);

        // Повертаємо повну URL-адресу файлу в інтернеті
        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string fileUrl)
    {
        // Витягуємо ім'я файлу з URL
        var uri = new Uri(fileUrl);
        var fileName = Path.GetFileName(uri.LocalPath);

        var blobClient = _containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}
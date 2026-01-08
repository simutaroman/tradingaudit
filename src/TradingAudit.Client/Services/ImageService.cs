using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using TradingAudit.Shared;

namespace TradingAudit.Client.Services;

public class ImageService : IImageService
{
    private readonly HttpClient _httpClient;

    public ImageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserImageDto>> GetAllImages()
    {
        // Токен додається автоматично через CustomAuthStateProvider, 
        // бо ми налаштували _http.DefaultRequestHeaders.Authorization там.
        return await _httpClient.GetFromJsonAsync<List<UserImageDto>>("api/UserImages")
               ?? new List<UserImageDto>();
    }

    public async Task<UserImageDto?> UploadImage(IBrowserFile file, string name)
    {
        // Ліміт на розмір файлу (наприклад 10 МБ)
        long maxFileSize = 10 * 1024 * 1024;

        using var content = new MultipartFormDataContent();

        // Додаємо ім'я
        content.Add(new StringContent(name), "name");

        // Додаємо файл (читаємо потік)
        // file.OpenReadStream(maxFileSize) відкриває потік для читання
        var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        content.Add(fileContent, "file", file.Name);

        var response = await _httpClient.PostAsync("api/UserImages", content);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserImageDto>();
        }

        return null;
    }

    public async Task DeleteImage(Guid id)
    {
        await _httpClient.DeleteAsync($"api/UserImages/{id}");
    }
}
using System.Net.Http.Json;
using TradingAudit.Shared.DTOs.Setups;

namespace TradingAudit.Client.Services;

public class SetupClient
{
    private readonly HttpClient _http;
    private const string BasePath = "api/setups";

    public SetupClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<SetupResponseDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<SetupResponseDto>>(BasePath)
               ?? new List<SetupResponseDto>();
    }

    public async Task<SetupResponseDto?> GetByIdAsync(Guid id)
    {
        var response = await _http.GetAsync($"{BasePath}/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SetupResponseDto>();
        }
        return null;
    }

    public async Task<SetupResponseDto?> CreateAsync(SetupCreateDto dto)
    {
        var response = await _http.PostAsJsonAsync(BasePath, dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SetupResponseDto>();
        }
        return null;
    }

    public async Task<SetupResponseDto?> UpdateAsync(Guid id, SetupResponseDto dto)
    {
        var response = await _http.PutAsJsonAsync($"{BasePath}/{id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SetupResponseDto>();
        }
        return null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"{BasePath}/{id}");
        return response.IsSuccessStatusCode;
    }
}
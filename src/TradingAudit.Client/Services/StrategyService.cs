using System.Net.Http.Json;
using System.Text.Json;
using TradingAudit.Shared.DTOs.Strategies;

namespace TradingAudit.Client.Services;

public class StrategyService : IStrategyService
{
    private readonly HttpClient _httpClient;

    public StrategyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TradingStrategyDto>> GetStrategiesAsync()
    {
        // Викликає GET api/strategies нашого контролера
        // Якщо сервер поверне 404 або помилку, тут треба обробити, 
        // але для старту (MVP) нехай кидає exception, побачимо в консолі.
        return await _httpClient.GetFromJsonAsync<List<TradingStrategyDto>>("api/strategies")
               ?? new List<TradingStrategyDto>();
    }

    public async Task<TradingStrategyDto?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TradingStrategyDto>($"api/strategies/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Guid> CreateStrategyAsync(TradingStrategyDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/strategies", dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error); // Викидаємо помилку (наприклад, про ліміти Free tier)
        }

        // Отримуємо ID створеної стратегії (з заголовка Location або тіла)
        // Оскільки ми повертали CreatedAtAction(..., newId), id буде в тілі, якщо ми його туди поклали,
        // але CreatedAtAction повертає об'єкт.
        // Давай спростимо: зчитаємо ID з JSON відповіді, якщо контролер повертає int/guid.
        // Але контролер повертає CreatedAtAction з об'єктом ID. 
        // Найпростіше: зчитати як int/Guid з тіла.

        var result = await response.Content.ReadFromJsonAsync<Guid>();
        return result;
    }

    public async Task<Guid> UpdateStrategyAsync(Guid id, TradingStrategyDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/strategies/{id}", dto);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error updating strategy");
        }

        // Сервер повертає { newVersionId = ... }
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("newVersionId").GetGuid();
    }
    public async Task<List<TradingStrategyDto>> GetHistoryAsync(Guid groupId)
    {
        return await _httpClient.GetFromJsonAsync<List<TradingStrategyDto>>($"api/strategies/history/{groupId}")
               ?? new List<TradingStrategyDto>();
    }
}
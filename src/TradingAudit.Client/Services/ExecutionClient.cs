using System.Net.Http.Json;
using TradingAudit.Shared.DTOs.Executions;

namespace TradingAudit.Client.Services;

public class ExecutionClient
{
    private readonly HttpClient _http;

    public ExecutionClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ExchangeOrderResponseDto>> GetUnlinkedOrdersAsync(string symbol)
    {
        return await _http.GetFromJsonAsync<List<ExchangeOrderResponseDto>>($"api/execution/unlinked/{symbol}")
               ?? new();
    }

    public async Task<ExecutionResponseDto?> LinkOrdersAsync(LinkOrdersRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("api/execution/link", request);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ExecutionResponseDto>();
        return null;
    }
}

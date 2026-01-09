using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace TradingAudit.Client.Services;

public class JwtAuthHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public JwtAuthHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Перехоплюємо запит і читаємо токен
        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

        if (!string.IsNullOrWhiteSpace(token))
        {
            // Додаємо хедер до поточного запиту
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Відправляємо запит далі
        return await base.SendAsync(request, cancellationToken);
    }
}

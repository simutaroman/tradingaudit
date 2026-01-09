using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace TradingAudit.Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // 1. Якщо токена немає - анонім
        if (string.IsNullOrWhiteSpace(token))
        {
            return GenerateAnonymous();
        }

        // 2. Парсимо claims
        var claims = ParseClaimsFromJwt(token);

        // 3. ПЕРЕВІРКА: Чи не прострочився токен?
        // Шукаємо claim "exp" (expiration time)
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
        if (expClaim != null)
        {
            var expValue = long.Parse(expClaim.Value);
            var expDate = DateTimeOffset.FromUnixTimeSeconds(expValue).UtcDateTime;

            if (expDate <= DateTime.UtcNow)
            {
                // Токен прострочений!
                await _localStorage.RemoveItemAsync("authToken"); // Видаляємо сміття
                return GenerateAnonymous(); // Повертаємо стан аноніма
            }
        }

        // 4. Якщо все ок - встановлюємо токен в хедер і повертаємо юзера
        //_http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(GenerateAnonymous());
        NotifyAuthenticationStateChanged(authState);
    }

    // Допоміжний метод для створення анонімного стану
    private AuthenticationState GenerateAnonymous()
    {
        //_http.DefaultRequestHeaders.Authorization = null; // Прибираємо токен з хедерів
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        if (string.IsNullOrEmpty(jwt)) return new List<Claim>();

        var parts = jwt.Split('.');
        if (parts.Length < 2) return new List<Claim>();

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null) return new List<Claim>();

        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
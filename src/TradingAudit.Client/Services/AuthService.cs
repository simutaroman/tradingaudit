using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TradingAudit.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient,
                       ILocalStorageService localStorage,
                       AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<string?> Login(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/identity/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            return "Login failed. Check your credentials.";
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

        // Додаємо перевірку IsNullOrEmpty
        if (string.IsNullOrEmpty(result?.AccessToken))
            return "No token received";

        await _localStorage.SetItemAsync("authToken", result.AccessToken);
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.AccessToken);

        return null;
    }

    public async Task Register(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/identity/register", new { email, password });
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Registration failed.");
        }
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    // Внутрішній клас для десеріалізації відповіді
    private class LoginResponse
    {
        // Вказуємо, що в JSON це поле називається 'accessToken'
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
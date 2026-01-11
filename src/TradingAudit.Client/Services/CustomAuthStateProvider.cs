using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TradingAudit.Shared; // Твоя DTO

namespace TradingAudit.Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    private const string AuthTokenKey = "authToken";

    // Кешуємо профіль, щоб не смикати API при кожному кліку
    private UserProfileDto? _cachedProfile;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // 1. Беремо токен
        var token = await _localStorage.GetItemAsync<string>(AuthTokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            return GenerateAnonymous();
        }

        // 2. Встановлюємо токен в хедер (щоб мати доступ до захищених ресурсів)
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // 3. Якщо у нас ще немає даних профілю - завантажуємо їх з сервера
            if (_cachedProfile == null)
            {
                // Цей запит пройде, бо ми вже встановили Header вище
                _cachedProfile = await _http.GetFromJsonAsync<UserProfileDto>("api/profile");
            }

            if (_cachedProfile != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _cachedProfile.Nickname ?? _cachedProfile.Email),
                    new Claim(ClaimTypes.Email, _cachedProfile.Email)
                };

                // Додаємо роль, якщо треба (можна розширити DTO)
                // if (_cachedProfile.IsAdmin) claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                var identity = new ClaimsIdentity(claims, "Bearer");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
        }
        catch (Exception)
        {
            // Якщо токен протух або сервер повернув 401
            await _localStorage.RemoveItemAsync(AuthTokenKey);
            _cachedProfile = null;
        }

        return GenerateAnonymous();
    }

    public async Task NotifyUserAuthentication(string token)
    {
        await _localStorage.SetItemAsync(AuthTokenKey, token);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Скидаємо кеш, щоб завантажити свіжі дані
        _cachedProfile = null;

        // Викликаємо оновлення стану (це запустить GetAuthenticationStateAsync)
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task NotifyUserLogout()
    {
        await _localStorage.RemoveItemAsync(AuthTokenKey);
        _http.DefaultRequestHeaders.Authorization = null;
        _cachedProfile = null;

        var authState = Task.FromResult(GenerateAnonymous());
        NotifyAuthenticationStateChanged(authState);
    }

    private AuthenticationState GenerateAnonymous()
    {
        _http.DefaultRequestHeaders.Authorization = null;
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
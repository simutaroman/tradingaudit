using TradingAudit.Shared; // Тут будуть DTO, але поки юзаємо прості моделі

namespace TradingAudit.Client.Services;

public interface IAuthService
{
    Task<string?> Login(string email, string password);
    Task Register(string email, string password);
    Task Logout();
}
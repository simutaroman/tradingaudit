using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Providers;

public interface IExchangeProvider
{
    string ExchangeName { get; }

    // Метод для отримання останніх ордерів разом із їхніми трейдами (Fills)
    Task<List<ExchangeOrder>> GetRecentOrdersAsync(string apiKey, string apiSecret, string userId, Guid keyId);
}

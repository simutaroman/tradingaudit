namespace TradingAudit.Server.Entities;

public class UserExchangeKey
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty; // Bybit, Binance

    // Ці поля будуть зберігати зашифровані дані
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;

    public string? Label { get; set; } // Назва аккаунту (напр. "Main Bybit")
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSyncAt { get; set; }
}
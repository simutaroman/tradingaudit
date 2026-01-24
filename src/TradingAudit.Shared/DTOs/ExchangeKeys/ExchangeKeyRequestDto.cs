namespace TradingAudit.Shared.DTOs.ExchangeKeys;

public class ExchangeKeyRequestDto
{
    public string ExchangeName { get; set; } = "Bybit";
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string? Label { get; set; }
}
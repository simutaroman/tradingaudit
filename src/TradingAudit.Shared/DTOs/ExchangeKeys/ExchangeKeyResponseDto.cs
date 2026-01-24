namespace TradingAudit.Shared.DTOs.ExchangeKeys;

public class ExchangeKeyResponseDto
{
    public Guid Id { get; set; }
    public string ExchangeName { get; set; } = string.Empty;
    public string MaskedApiKey { get; set; } = string.Empty;
    public string? Label { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastSyncAt { get; set; }
}
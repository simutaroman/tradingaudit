namespace TradingAudit.Shared.DTOs.Executions;

public class ExecutionResponseDto
{
    public Guid Id { get; set; }
    public Guid SetupId { get; set; }

    // Фінанси
    public decimal RealizedPnL { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalFunding { get; set; }
    public decimal NetPnL { get; set; }
    public decimal ROI { get; set; }

    // Ціни
    public decimal AvgEntryPrice { get; set; }
    public decimal AvgExitPrice { get; set; }
    public decimal SlippagePoints { get; set; }
    public decimal SlippagePercent { get; set; }

    // Таймінг
    public DateTime? OpenTime { get; set; }
    public DateTime? CloseTime { get; set; }
    public double? DurationMinutes => CloseTime.HasValue && OpenTime.HasValue
        ? (CloseTime.Value - OpenTime.Value).TotalMinutes
        : null;

    // Список ордерів для деталізації
    public List<ExchangeOrderResponseDto> Orders { get; set; } = new();
}

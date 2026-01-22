using TradingAudit.Shared.DTOs.Executions;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Shared.DTOs.Setups;

public class SetupResponseDto
{
    public Guid Id { get; set; }
    public Guid StrategyId { get; set; }
    public string StrategyName { get; set; } = string.Empty;
    public int StrategyVersion { get; set; }

    public string Symbol { get; set; } = string.Empty;
    public TradeDirection Direction { get; set; }
    public string Timeframe { get; set; } = string.Empty;

    public decimal EntryPrice { get; set; }
    public decimal StopLoss { get; set; }
    public decimal TakeProfit { get; set; }
    public decimal? PositionSize { get; set; }

    public string? TradingThesis { get; set; }
    public string? Mood { get; set; }
    public int ConfidenceLevel { get; set; }
    public SetupLifecycleStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<SetupImageDto> Images { get; set; } = new();
    public List<SetupChecklistItemDto> Checklist { get; set; } = new();

    public ExecutionResponseDto? Execution { get; set; }
}
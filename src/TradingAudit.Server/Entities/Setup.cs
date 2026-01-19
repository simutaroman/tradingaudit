using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Entities;

public class Setup
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public Guid StrategyId { get; set; }
    public TradingStrategy? Strategy { get; set; }

    public string Symbol { get; set; } = string.Empty;
    public TradeDirection Direction { get; set; } // Enum
    public string Timeframe { get; set; } = string.Empty;

    public decimal EntryPrice { get; set; }
    public decimal StopLoss { get; set; }
    public decimal TakeProfit { get; set; }
    public decimal? PositionSize { get; set; }

    public string? TradingThesis { get; set; }
    public string? Mood { get; set; } // Залишаємо STRING (теги або текст)
    public int ConfidenceLevel { get; set; }

    public SetupLifecycleStatus Status { get; set; } = SetupLifecycleStatus.Draft; // Enum
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<SetupChecklistItem> ChecklistItems { get; set; } = new();
    public List<SetupImage> Images { get; set; } = new();
}
using TradingAudit.Shared.Enums;

namespace TradingAudit.Shared.DTOs.Strategies;

public class TradingStrategyDto
{
    public Guid? Id { get; set; } // Null при створенні
    public Guid? GroupId { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Risk
    public decimal DefaultRiskAmount { get; set; }
    public decimal MinRiskRewardRatio { get; set; }

    // Scope (наприклад "15m, 1h")
    public string Timeframes { get; set; } = string.Empty;
    public string AssetScope { get; set; } = string.Empty;

    public StrategyLifecycleStatus LifecycleStatus { get; set; }

    // Collections
    public List<string> ImageUrls { get; set; } = new(); // Обкладинки стратегії
    public List<StrategyRuleDto> Rules { get; set; } = new();

    public bool CreateNewVersion { get; set; } = false;
}
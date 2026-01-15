using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Entities;

public class TradingStrategy
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Owner
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    // Versioning Identity
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // GroupId об'єднує всі ітерації однієї стратегії.
    // У v1 GroupId == Id. У v2 GroupId залишається від v1.
    public Guid GroupId { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    // Risk Parameters
    public decimal DefaultRiskAmount { get; set; } // $50
    public decimal MinRiskRewardRatio { get; set; } // 2.0

    // Scope (зберігаємо списком через кому, наприклад "15m,1h")
    public string Timeframes { get; set; } = string.Empty;
    public string AssetScope { get; set; } = string.Empty;

    public StrategyLifecycleStatus LifecycleStatus { get; set; } = StrategyLifecycleStatus.Development;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Collections
    public ICollection<StrategyImage> Images { get; set; } = new List<StrategyImage>();
    public ICollection<StrategyRule> Rules { get; set; } = new List<StrategyRule>();
}
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Entities;

public class StrategyRule
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid StrategyId { get; set; }
    public TradingStrategy? Strategy { get; set; }

    public StrategyRuleType Type { get; set; } // Entry or Management
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
    public int OrderIndex { get; set; }

    // Опціональний параметр (наприклад "3.0" для R)
    public decimal? LogicParameter { get; set; }

    public ICollection<StrategyRuleImage> Images { get; set; } = new List<StrategyRuleImage>();
}
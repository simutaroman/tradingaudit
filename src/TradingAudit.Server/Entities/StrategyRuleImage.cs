namespace TradingAudit.Server.Entities;

public class StrategyRuleImage: ImageBase
{
    public Guid StrategyRuleId { get; set; }
    public StrategyRule? StrategyRule { get; set; }
}

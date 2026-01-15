namespace TradingAudit.Server.Entities;

public class StrategyRuleImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; } // "Good Example", "Bad Example"

    public Guid StrategyRuleId { get; set; }
    public StrategyRule? StrategyRule { get; set; }
}

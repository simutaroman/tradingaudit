namespace TradingAudit.Server.Entities;

public class StrategyImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid StrategyId { get; set; }
    public TradingStrategy? Strategy { get; set; }
}

namespace TradingAudit.Server.Entities;

public class StrategyImage : ImageBase
{
    public Guid StrategyId { get; set; }
    public TradingStrategy? Strategy { get; set; }
}

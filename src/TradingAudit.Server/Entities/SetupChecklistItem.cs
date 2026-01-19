namespace TradingAudit.Server.Entities;

public class SetupChecklistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SetupId { get; set; }
    public Setup? Setup { get; set; }

    public Guid StrategyRuleId { get; set; }
    public StrategyRule? StrategyRule { get; set; }

    public int OrderIndex { get; set; } // Копіюємо для збереження порядку
    public bool IsMet { get; set; }
    public string? Comment { get; set; }

    public List<SetupChecklistItemImage> Images { get; set; } = new();
}

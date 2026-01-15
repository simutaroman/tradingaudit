using TradingAudit.Shared.Enums;

namespace TradingAudit.Shared.DTOs.Strategies;

public class StrategyRuleDto
{
    public Guid? Id { get; set; } // Null для нових правил
    public StrategyRuleType Type { get; set; } // Entry / Management
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
    public int OrderIndex { get; set; }
    public decimal? LogicParameter { get; set; }

    // Список URL картинок (для простоти приймаємо список рядків)
    public List<string> ImageUrls { get; set; } = new();
}
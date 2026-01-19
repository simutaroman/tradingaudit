namespace TradingAudit.Shared.DTOs.Setups;

public class SetupChecklistItemDto
{
    public Guid Id { get; set; }
    public Guid StrategyRuleId { get; set; } // ПОВЕРНУТО

    // Дані з самої стратегії для відображення в UI
    public string RuleTitle { get; set; } = string.Empty;
    public string? RuleDescription { get; set; }
    public bool IsMandatory { get; set; } // ВИПРАВЛЕНО (відповідно до Entity)
    public int OrderIndex { get; set; }

    // Дані сетапу
    public bool IsMet { get; set; }
    public string? Comment { get; set; }
    public List<SetupImageDto> Images { get; set; } = new();
}

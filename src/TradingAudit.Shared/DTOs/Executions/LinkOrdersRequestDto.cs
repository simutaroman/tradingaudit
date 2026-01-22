namespace TradingAudit.Shared.DTOs.Executions;

public class LinkOrdersRequestDto
{
    public Guid SetupId { get; set; }
    public List<Guid> OrderIds { get; set; } = new();
}
namespace TradingAudit.Shared.DTOs.Setups;

public class SetupImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
}

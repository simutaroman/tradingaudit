namespace TradingAudit.Server.Entities;

public abstract class ImageBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
}
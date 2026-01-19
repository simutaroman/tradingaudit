namespace TradingAudit.Server.Entities;

public class SetupChecklistItemImage : ImageBase
{
    public Guid SetupChecklistItemId { get; set; }
    public SetupChecklistItem? SetupChecklistItem { get; set; }
}

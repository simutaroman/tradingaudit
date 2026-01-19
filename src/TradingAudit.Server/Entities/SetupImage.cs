namespace TradingAudit.Server.Entities;

public class SetupImage : ImageBase
{
    public Guid SetupId { get; set; }
    public Setup? Setup { get; set; }
}

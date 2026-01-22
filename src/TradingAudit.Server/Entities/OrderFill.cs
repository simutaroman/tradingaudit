namespace TradingAudit.Server.Entities;

public class OrderFill
{
    public Guid Id { get; set; }
    public string ExternalFillId { get; set; } = null!; // TradeId з біржі

    public Guid ExchangeOrderId { get; set; }
    public ExchangeOrder ExchangeOrder { get; set; } = null!;

    public decimal Price { get; set; }
    public decimal Qty { get; set; }

    public decimal Commission { get; set; }
    public string CommissionAsset { get; set; } = "USDT";

    public DateTime TradeTime { get; set; }
}

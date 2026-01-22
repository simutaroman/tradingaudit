namespace TradingAudit.Shared.DTOs.Executions;

public class OrderFillDto
{
    public string ExternalFillId { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Qty { get; set; }
    public decimal Commission { get; set; }
    public string CommissionAsset { get; set; } = "USDT";
    public DateTime TradeTime { get; set; }
}
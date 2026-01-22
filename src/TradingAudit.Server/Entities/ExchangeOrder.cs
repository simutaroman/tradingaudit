namespace TradingAudit.Server.Entities;

public class ExchangeOrder
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;

    // ID з біржі (наприклад, Binance OrderId)
    public string ExternalOrderId { get; set; } = null!;
    public string Symbol { get; set; } = null!;

    public string Side { get; set; } = null!; // Buy, Sell
    public string Type { get; set; } = null!; // Limit, Market, StopLoss, etc.
    public string Status { get; set; } = null!; // Filled, Canceled, New

    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal ExecutedAmount { get; set; }

    public DateTime OrderTime { get; set; }

    // Зв'язок із Execution (може бути порожнім, поки не прив'язали)
    public Guid? ExecutionId { get; set; }
    public Execution? Execution { get; set; }

    public List<OrderFill> Fills { get; set; } = new();
}

namespace TradingAudit.Shared.DTOs.Executions;

public class ExchangeOrderResponseDto
{
    public Guid Id { get; set; }
    public string ExternalOrderId { get; set; } = null!;
    public string Symbol { get; set; } = null!;
    public string Side { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;

    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal ExecutedAmount { get; set; }
    public DateTime OrderTime { get; set; }

    public List<OrderFillDto> Fills { get; set; } = new();
}
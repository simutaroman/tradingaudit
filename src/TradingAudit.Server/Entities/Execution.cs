namespace TradingAudit.Server.Entities;

public class Execution
{
    public Guid Id { get; set; }

    // Зв'язок 1:1 із Сетапом
    public Guid SetupId { get; set; }
    public Setup Setup { get; set; } = null!;

    // Обчислювальні фінансові поля
    public decimal RealizedPnL { get; set; }    // Gross (без комісій)
    public decimal NetPnL { get; set; }         // Net (PnL - Comm + Funding)
    public decimal TotalCommission { get; set; }
    public decimal TotalFunding { get; set; }

    public decimal ROI { get; set; }            // %

    // Аналітичні цінові поля
    public decimal AvgEntryPrice { get; set; }
    public decimal AvgExitPrice { get; set; }

    public decimal SlippagePoints { get; set; } // Відхилення від плану в пунктах
    public decimal SlippagePercent { get; set; }

    // Таймінг
    public DateTime? OpenTime { get; set; }     // Час першого Fill
    public DateTime? CloseTime { get; set; }    // Час останнього Fill
    public DateTime? BreakevenTime { get; set; } // Коли PnL став > 0 стабільно

    // Список прив'язаних ордерів
    public List<ExchangeOrder> Orders { get; set; } = new();
}

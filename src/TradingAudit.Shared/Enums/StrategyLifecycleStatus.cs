namespace TradingAudit.Shared.Enums;

public enum StrategyLifecycleStatus
{
    Development = 0, // Розробка та бектести
    Active = 1,      // Активна торгівля
    Paused = 2,      // Тимчасово зупинена
    Archived = 3     // Архів (старі версії)
}

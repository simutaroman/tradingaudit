using TradingAudit.Shared.DTOs.Strategies;
using TradingAudit.Shared.Enums; // Для SubscriptionTier якщо треба, але тут ми передаємо DTO

namespace TradingAudit.Client.Services;

public interface IStrategyService
{
    Task<List<TradingStrategyDto>> GetStrategiesAsync();
    Task<TradingStrategyDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateStrategyAsync(TradingStrategyDto dto);
    Task<Guid> UpdateStrategyAsync(Guid id, TradingStrategyDto dto);
    Task<List<TradingStrategyDto>> GetHistoryAsync(Guid groupId);
}
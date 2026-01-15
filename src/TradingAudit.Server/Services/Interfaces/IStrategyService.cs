using TradingAudit.Shared.DTOs.Strategies;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Services.Interfaces;

public interface IStrategyService
{
    Task<List<TradingStrategyDto>> GetActiveStrategiesAsync(string userId);
    Task<TradingStrategyDto?> GetByIdAsync(string userId, Guid id);

    // Повертає ID створеної стратегії
    Task<Guid> CreateAsync(string userId, SubscriptionTier tier, TradingStrategyDto dto);

    // Версійність: повертає ID нової версії
    Task<Guid> UpdateAsync(string userId, Guid currentId, TradingStrategyDto dto);

    Task DeleteAsync(string userId, Guid id);

    Task<List<TradingStrategyDto>> GetHistoryAsync(string userId, Guid groupId);
}
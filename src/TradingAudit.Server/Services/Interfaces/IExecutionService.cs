namespace TradingAudit.Server.Services.Interfaces;

using TradingAudit.Shared.DTOs.Executions;

public interface IExecutionService
{
    // Прив'язка ордерів до сетапу та розрахунок статистики
    Task<ExecutionResponseDto> LinkOrdersToSetupAsync(string userId, LinkOrdersRequestDto request);

    // Отримання списку ордерів, які ще не прив'язані до жодного виконання
    Task<List<ExchangeOrderResponseDto>> GetUnlinkedOrdersAsync(string userId, string symbol);

    // Отримання поточного виконання для сетапу
    Task<ExecutionResponseDto?> GetBySetupIdAsync(Guid setupId, string userId);
}
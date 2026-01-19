using TradingAudit.Shared.DTOs.Setups;

namespace TradingAudit.Server.Services.Interfaces;

public interface ISetupService
{
    Task<SetupResponseDto> CreateAsync(string userId, SetupCreateDto dto);
    Task<SetupResponseDto> GetByIdAsync(string userId, Guid id);
    Task<List<SetupResponseDto>> GetAllAsync(string userId);
    Task<SetupResponseDto> UpdateAsync(string userId, Guid setupId, SetupResponseDto dto);
    Task DeleteAsync(string userId, Guid id);
}
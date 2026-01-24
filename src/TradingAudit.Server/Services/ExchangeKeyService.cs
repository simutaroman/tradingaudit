using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Shared.DTOs.ExchangeKeys;

namespace TradingAudit.Server.Services;

public interface IExchangeKeyService
{
    Task<List<ExchangeKeyResponseDto>> GetUserKeysAsync(string userId);
    Task<bool> AddKeyAsync(string userId, ExchangeKeyRequestDto dto);
    Task<bool> DeleteKeyAsync(Guid id, string userId);
}

public class ExchangeKeyService : IExchangeKeyService
{
    private readonly ApplicationDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public ExchangeKeyService(ApplicationDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    public async Task<List<ExchangeKeyResponseDto>> GetUserKeysAsync(string userId)
    {
        var keys = await _context.UserExchangeKeys
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return keys.Select(k => new ExchangeKeyResponseDto
        {
            Id = k.Id,
            ExchangeName = k.ExchangeName,
            Label = k.Label,
            IsActive = k.IsActive,
            LastSyncAt = k.LastSyncAt,
            MaskedApiKey = k.ApiKey.Length > 10
                ? $"{k.ApiKey[..4]}...{k.ApiKey[^4..]}"
                : "****"
        }).ToList();
    }

    public async Task<bool> AddKeyAsync(string userId, ExchangeKeyRequestDto dto)
    {
        var key = new UserExchangeKey
        {
            UserId = userId,
            ExchangeName = dto.ExchangeName,
            ApiKey = dto.ApiKey, // Можна теж шифрувати, якщо хочеться максимальної приватності
            ApiSecret = _encryptionService.Encrypt(dto.ApiSecret),
            Label = dto.Label,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserExchangeKeys.Add(key);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteKeyAsync(Guid id, string userId)
    {
        var key = await _context.UserExchangeKeys
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (key == null) return false;

        _context.UserExchangeKeys.Remove(key);
        return await _context.SaveChangesAsync() > 0;
    }
}
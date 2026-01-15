using Mapster;
using MapsterMapper; // <--- Інтерфейс IMapper живе тут
using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.DTOs.Strategies;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Services;

public class StrategyService : IStrategyService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper; // <--- Інжектимо маппер
    private readonly TypeAdapterConfig _config; // <--- Інжектимо конфіг (для ProjectToType)

    public StrategyService(ApplicationDbContext context, IMapper mapper, TypeAdapterConfig config)
    {
        _context = context;
        _mapper = mapper;
        _config = config;
    }

    public async Task<List<TradingStrategyDto>> GetActiveStrategiesAsync(string userId)
    {
        return await _context.TradingStrategies
            .Where(s => s.UserId == userId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            // Явно передаємо конфіг, щоб EF Core знав, як мапити ImageUrls
            .ProjectToType<TradingStrategyDto>(_config)
            .ToListAsync();
    }

    public async Task<TradingStrategyDto?> GetByIdAsync(string userId, Guid id)
    {
        return await _context.TradingStrategies
            .Where(s => s.Id == id && s.UserId == userId)
            .ProjectToType<TradingStrategyDto>(_config)
            .FirstOrDefaultAsync();
    }

    public async Task<Guid> CreateAsync(string userId, SubscriptionTier tier, TradingStrategyDto dto)
    {
        // ... (перевірка лімітів tier залишається такою ж) ...
        if (tier == SubscriptionTier.Free)
        {
            var activeCount = await _context.TradingStrategies
               .Where(s => s.UserId == userId && s.IsActive && s.LifecycleStatus != StrategyLifecycleStatus.Archived)
               .Select(s => s.GroupId).Distinct().CountAsync();
            if (activeCount >= 1) throw new InvalidOperationException("Free tier limit reached.");
        }

        // 1. Explicit Mapping через сервіс
        var strategy = _mapper.Map<TradingStrategy>(dto);

        // 2. Дозаповнюємо
        strategy.Id = Guid.NewGuid();
        strategy.UserId = userId;
        strategy.GroupId = strategy.Id;
        strategy.Version = 1;
        strategy.IsActive = true;
        strategy.CreatedAt = DateTime.UtcNow;

        _context.TradingStrategies.Add(strategy);
        await _context.SaveChangesAsync();

        return strategy.Id;
    }

    public async Task<Guid> UpdateAsync(string userId, Guid currentId, TradingStrategyDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Отримуємо поточну стратегію з усіма зв'язками
            // Include потрібні для сценарію In-Place Update (щоб видалити старі правила)
            // та для сценарію Copy (щоб мати доступ до GroupId)
            var existingStrategy = await _context.TradingStrategies
                .Include(s => s.Images)
                .Include(s => s.Rules).ThenInclude(r => r.Images)
                .FirstOrDefaultAsync(s => s.Id == currentId && s.UserId == userId);

            if (existingStrategy == null) throw new KeyNotFoundException("Strategy not found");

            // === ГІЛКА 1: СТВОРЕННЯ НОВОЇ ВЕРСІЇ (IMMUTABILITY / FORKING) ===
            if (dto.CreateNewVersion)
            {
                // 1. Знаходимо найвищу версію в цій групі (щоб зробити +1)
                var maxVersion = await _context.TradingStrategies
                    .Where(s => s.GroupId == existingStrategy.GroupId)
                    .MaxAsync(s => s.Version);

                // 2. Деактивуємо ту версію, яка ЗАРАЗ є активною в цій групі.
                // (Це може бути existingStrategy, а може бути й інша версія, якщо ми редагуємо архів)
                await _context.TradingStrategies
                    .Where(s => s.GroupId == existingStrategy.GroupId && s.IsActive)
                    .ExecuteUpdateAsync(calls => calls.SetProperty(s => s.IsActive, false));

                // 3. Мапимо DTO в нову сутність
                var newStrategy = _mapper.Map<TradingStrategy>(dto);

                // 4. Налаштовуємо системні поля
                newStrategy.Id = Guid.NewGuid();                // Абсолютно новий ID
                newStrategy.UserId = userId;
                newStrategy.GroupId = existingStrategy.GroupId; // Група та сама
                newStrategy.Version = maxVersion + 1;           // Версія = Максимальна + 1
                newStrategy.IsActive = true;                    // Нова стає активною
                newStrategy.CreatedAt = DateTime.UtcNow;

                // 5. DEEP COPY: Генеруємо нові ID для всіх вкладених об'єктів
                // Це критично, інакше EF спробує вставити правила зі старими ID і впаде

                // а) Обкладинки стратегії
                foreach (var img in newStrategy.Images)
                {
                    img.Id = Guid.NewGuid();
                    img.StrategyId = newStrategy.Id; // Прив'язуємо до нової стратегії
                }

                // б) Правила та їх картинки
                foreach (var rule in newStrategy.Rules)
                {
                    rule.Id = Guid.NewGuid();        // Новий ID правила
                    rule.StrategyId = newStrategy.Id; // Прив'язуємо до нової стратегії

                    foreach (var ruleImg in rule.Images)
                    {
                        ruleImg.Id = Guid.NewGuid();     // Новий ID картинки
                        ruleImg.StrategyRuleId = rule.Id; // Прив'язуємо до НОВОГО правила
                    }
                }

                _context.TradingStrategies.Add(newStrategy);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return newStrategy.Id;
            }

            // === ГІЛКА 2: ОНОВЛЕННЯ ПОТОЧНОЇ ВЕРСІЇ (IN-PLACE UPDATE) ===
            else
            {
                // Захист: Не можна змінювати архів "заднім числом".
                // Тільки активну версію можна правити "на льоту".
                if (!existingStrategy.IsActive)
                {
                    throw new InvalidOperationException("Архівну версію не можна редагувати. Встановіть 'Створити нову версію'.");
                }

                // 1. Оновлюємо прості поля (Scalar properties)
                existingStrategy.Name = dto.Name;
                existingStrategy.Description = dto.Description;
                existingStrategy.DefaultRiskAmount = dto.DefaultRiskAmount;
                existingStrategy.MinRiskRewardRatio = dto.MinRiskRewardRatio;
                existingStrategy.Timeframes = dto.Timeframes;
                existingStrategy.AssetScope = dto.AssetScope;
                // Version не міняємо
                // GroupId не міняємо

                // 2. Оновлення колекцій методом "Wipe & Replace"
                // Видаляємо старі записи з БД
                _context.StrategyImages.RemoveRange(existingStrategy.Images);
                _context.StrategyRules.RemoveRange(existingStrategy.Rules);

                // Створюємо тимчасовий об'єкт через мапер, щоб отримати колекції сутностей з DTO
                var tempMapped = _mapper.Map<TradingStrategy>(dto);

                // Переносимо нові колекції в existingStrategy, генеруючи нові ID
                foreach (var img in tempMapped.Images)
                {
                    img.Id = Guid.NewGuid();
                    img.StrategyId = existingStrategy.Id;
                    _context.StrategyImages.Add(img);
                }

                foreach (var rule in tempMapped.Rules)
                {
                    rule.Id = Guid.NewGuid();
                    rule.StrategyId = existingStrategy.Id;

                    foreach (var rImg in rule.Images)
                    {
                        rImg.Id = Guid.NewGuid();
                        rImg.StrategyRuleId = rule.Id;
                    }
                    _context.StrategyRules.Add(rule);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return existingStrategy.Id; // ID не змінився
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ... DeleteAsync без змін ...
    public async Task DeleteAsync(string userId, Guid id)
    {
        await _context.TradingStrategies
           .Where(s => s.Id == id && s.UserId == userId)
           .ExecuteUpdateAsync(calls =>
               calls.SetProperty(s => s.IsActive, false)
                    .SetProperty(s => s.LifecycleStatus, StrategyLifecycleStatus.Archived));
    }

    public async Task<List<TradingStrategyDto>> GetHistoryAsync(string userId, Guid groupId)
    {
        return await _context.TradingStrategies
            .Where(s => s.UserId == userId && s.GroupId == groupId)
            .OrderByDescending(s => s.Version) // Спочатку нові
            .ProjectToType<TradingStrategyDto>(_config)
            .ToListAsync();
    }
}
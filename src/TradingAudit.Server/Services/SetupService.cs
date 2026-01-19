using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.DTOs.Setups;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Services;

public class SetupService: ISetupService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SetupService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // === СТВОРЕННЯ СЕТАПУ (ПЛАНУ) ===
    public async Task<SetupResponseDto> CreateAsync(string userId, SetupCreateDto dto)
    {
        // 1. Отримуємо стратегію з правилами для генерації чек-ліста
        var strategy = await _context.TradingStrategies
            .Include(s => s.Rules)
            .FirstOrDefaultAsync(s => s.Id == dto.StrategyId && s.UserId == userId);

        if (strategy == null) throw new KeyNotFoundException("Strategy not found");

        // 2. Мапимо базові поля з DTO в Entity
        var setup = _mapper.Map<Setup>(dto);
        setup.Id = Guid.NewGuid();
        setup.UserId = userId;
        setup.Status = SetupLifecycleStatus.Draft;
        setup.CreatedAt = DateTime.UtcNow;

        // 3. Генерація пунктів чек-ліста на основі правил стратегії
        foreach (var rule in strategy.Rules.OrderBy(r => r.OrderIndex))
        {
            setup.ChecklistItems.Add(new SetupChecklistItem
            {
                Id = Guid.NewGuid(),
                SetupId = setup.Id,
                StrategyRuleId = rule.Id,
                OrderIndex = rule.OrderIndex,
                IsMet = false,
                Comment = string.Empty
            });
        }

        _context.Setups.Add(setup);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(userId, setup.Id);
    }

    // === ОТРИМАННЯ ОДНОГО СЕТАПУ ===
    public async Task<SetupResponseDto> GetByIdAsync(string userId, Guid id)
    {
        // Використовуємо AsNoTracking(), щоб уникнути проблем із кешуванням об'єктів у пам'яті
        // після операцій Wipe & Replace в UpdateAsync.
        var setup = await _context.Setups
            .AsNoTracking()
            .Include(s => s.Strategy)
            .Include(s => s.Images)
            .Include(s => s.ChecklistItems.OrderBy(ci => ci.OrderIndex)) // Сортуємо пункти відразу
                .ThenInclude(ci => ci.StrategyRule)
            .Include(s => s.ChecklistItems)
                .ThenInclude(ci => ci.Images)
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (setup == null) throw new KeyNotFoundException("Setup not found");

        // Мапимо в DTO. Назви правил та стратегії підтягнуться завдяки Include вище.
        return _mapper.Map<SetupResponseDto>(setup);
    }

    // === ОТРИМАННЯ ВСІХ СЕТАПІВ КОРИСТУВАЧА ===
    public async Task<List<SetupResponseDto>> GetAllAsync(string userId)
    {
        var setups = await _context.Setups
            .Include(s => s.Strategy)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<SetupResponseDto>>(setups);
    }

    // === ОНОВЛЕННЯ СЕТАПУ (ПРОЦЕС ТОРГІВЛІ) ===
    public async Task<SetupResponseDto> UpdateAsync(string userId, Guid setupId, SetupResponseDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Отримуємо існуючий сетап (тільки скалярні поля, без Includes для видалення)
            var existingSetup = await _context.Setups
                .Include(s => s.Images)
                .Include(s => s.ChecklistItems)
                    .ThenInclude(ci => ci.Images)
                .FirstOrDefaultAsync(s => s.Id == setupId && s.UserId == userId);

            if (existingSetup == null) throw new KeyNotFoundException("Setup not found");

            // 2. Оновлюємо основні поля
            // id не міняємо
            // userId не міняємо
            // strategyId не міняємо
            existingSetup.ConfidenceLevel = dto.ConfidenceLevel;
            existingSetup.CreatedAt  = dto.CreatedAt;
            existingSetup.Direction = dto.Direction;
            existingSetup.EntryPrice = dto.EntryPrice;
            existingSetup.Mood = dto.Mood;
            existingSetup.PositionSize = dto.PositionSize;
            existingSetup.Status = dto.Status;
            existingSetup.StopLoss = dto.StopLoss;
            existingSetup.Symbol = dto.Symbol;
            existingSetup.TakeProfit = dto.TakeProfit;
            existingSetup.Timeframe = dto.Timeframe;
            existingSetup.TradingThesis = dto.TradingThesis;

            // 3. ВИДАЛЕННЯ СТАРИХ ДАНИХ (Прямим запитом або через контекст)
            // Видаляємо всі картинки та ітеми, що належать цьому сетапу
            var oldItems = _context.SetupChecklistItems.Where(i => i.SetupId == setupId);
            var oldItemImages = _context.SetupChecklistItemImages.Where(img => oldItems.Any(i => i.Id == img.SetupChecklistItemId));
            var oldSetupImages = _context.SetupImages.Where(img => img.SetupId == setupId);

            //_context.SetupChecklistItemImages.RemoveRange(oldItemImages);
            _context.SetupChecklistItems.RemoveRange(oldItems);
            _context.SetupImages.RemoveRange(oldSetupImages);

            // Зберігаємо видалення, щоб очистити базу перед вставкою
            //await _context.SaveChangesAsync();

            // 4. ДОДАВАННЯ НОВИХ ДАНИХ
            var tempMapped = _mapper.Map<Setup>(dto);

            foreach (var img in tempMapped.Images)
            {
                img.Id = Guid.NewGuid();
                img.SetupId = setupId;
                _context.SetupImages.Add(img);
            }

            foreach (var item in tempMapped.ChecklistItems)
            {
                var newItemId = Guid.NewGuid();
                item.Id = newItemId;
                item.SetupId = setupId;

                foreach (var itemImg in item.Images)
                {
                    itemImg.Id = Guid.NewGuid();
                    itemImg.SetupChecklistItemId = newItemId;
                }
                _context.SetupChecklistItems.Add(item);
            }

            // Остаточне збереження нових даних
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Очищуємо трекер перед завантаженням результату
            _context.ChangeTracker.Clear();

            return await GetByIdAsync(userId, setupId);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // === ВИДАЛЕННЯ ===
    public async Task DeleteAsync(string userId, Guid id)
    {
        var setup = await _context.Setups
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

        if (setup != null)
        {
            _context.Setups.Remove(setup);
            await _context.SaveChangesAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Providers;
using TradingAudit.Server.Services;

namespace TradingAudit.Server.BackgroundServices;

public class ExchangeSyncWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExchangeSyncWorker> _logger;

    public ExchangeSyncWorker(IServiceScopeFactory scopeFactory, ILogger<ExchangeSyncWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Exchange Sync Worker стартував.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var encryption = scope.ServiceProvider.GetRequiredService<IEncryptionService>();
                var providers = scope.ServiceProvider.GetServices<IExchangeProvider>();

                // 1. Беремо всі активні ключі
                var keys = await dbContext.UserExchangeKeys.Where(k => k.IsActive).ToListAsync(stoppingToken);

                foreach (var key in keys)
                {
                    // 2. Шукаємо потрібний провайдер (Bybit, Binance тощо)
                    var provider = providers.FirstOrDefault(p => p.ExchangeName == key.ExchangeName);
                    if (provider == null)
                    {
                        _logger.LogWarning("Провайдер для {Exchange} не знайдений", key.ExchangeName);
                        continue;
                    }

                    _logger.LogInformation("Синхронізація {Exchange} для користувача {UserId}...", key.ExchangeName, key.UserId);

                    // 3. Дешифруємо секрет
                    var decryptedSecret = encryption.Decrypt(key.ApiSecret);

                    // 4. Отримуємо дані з біржі
                    var orders = await provider.GetRecentOrdersAsync(key.ApiKey, decryptedSecret, key.UserId, key.Id);

                    foreach (var order in orders)
                    {
                        // 5. Перевіряємо унікальність за ExternalOrderId та UserId
                        var exists = await dbContext.ExchangeOrders
                            .AnyAsync(o => o.ExternalOrderId == order.ExternalOrderId && o.UserId == key.UserId, stoppingToken);

                        if (!exists)
                        {
                            dbContext.ExchangeOrders.Add(order);
                        }
                    }

                    key.LastSyncAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка під час циклу синхронізації.");
            }

            // Чекаємо 15 хвилин
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
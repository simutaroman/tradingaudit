using Bybit.Net.Clients;
using Bybit.Net.Enums;
using Bybit.Net.Objects.Models.V5;
using CryptoExchange.Net.Authentication;
using Microsoft.Extensions.Logging;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Providers;

public class BybitProvider : IExchangeProvider
{
    private readonly ILogger<BybitProvider> _logger;
    public string ExchangeName => "Bybit";

    public BybitProvider(ILogger<BybitProvider> logger)
    {
        _logger = logger;
    }

    public async Task<List<ExchangeOrder>> GetRecentOrdersAsync(string apiKey, string apiSecret, string userId, Guid keyId)
    {
        try
        {
            var client = new BybitRestClient(options =>
            {
                options.ApiCredentials = new ApiCredentials(apiKey, apiSecret);
            });

            // 1. Отримуємо історію ордерів через прямі параметри
            // Згідно з твоїм визначенням: GetOrderHistoryAsync(Category category, ...)
            var orderResult = await client.V5Api.Trading.GetOrderHistoryAsync(
                category: Category.Linear,
                limit: 50
            );

            if (!orderResult.Success)
            {
                _logger.LogError("Bybit API Error (Orders): {Error}", orderResult.Error);
                return new List<ExchangeOrder>();
            }

            // 2. Отримуємо трейди (аналогічно, перевір метадані GetUserTradesAsync, 
            // скоріш за все там теж прямі параметри)
            var fillsResult = await client.V5Api.Trading.GetUserTradesAsync(
                category: Category.Linear,
                limit: 100
            );

            var allFills = fillsResult.Success  ? fillsResult.Data.List.ToList()  : new List<BybitUserTrade>();

            // 3. Мапування (Entity <-> BybitOrder)
            return orderResult.Data.List.Select(o => new ExchangeOrder
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserExchangeKeyId = keyId,
                ExternalOrderId = o.OrderId,
                Symbol = o.Symbol,
                Side = o.Side.ToString().ToUpper(),
                Type = o.OrderType.ToString().ToUpper(),
                Status = o.Status.ToString().ToUpper(),

                Price = o.Price ?? 0,
                Amount = o.Quantity,          // Було o.Qty
                ExecutedAmount = o.QuantityFilled ?? 0, // Було o.FilledQty
                OrderTime = o.CreateTime,

                Fills = allFills
                    .Where(f => f.OrderId == o.OrderId)
                    .Select(f => new OrderFill
                    {
                        Id = Guid.NewGuid(),
                        ExternalFillId = f.TradeId,
                        Price = f.Price,
                        Qty = f.Quantity,
                        Commission = f.Fee ?? 0m,
                        CommissionAsset = f.FeeAsset ?? "USDT",
                        TradeTime = f.Timestamp
                    }).ToList()
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Критична помилка BybitProvider для {UserId}", userId);
            return new List<ExchangeOrder>();
        }
    }
}
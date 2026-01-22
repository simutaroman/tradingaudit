using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.DTOs.Executions;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Services
{
    public class ExecutionService : IExecutionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ExecutionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExecutionResponseDto> LinkOrdersToSetupAsync(string userId, LinkOrdersRequestDto request)
        {
            // 1. Отримуємо сетап та вибрані ордери з їхніми трейдами (Fills)
            var setup = await _context.Setups
                .Include(s => s.Execution)
                .FirstOrDefaultAsync(s => s.Id == request.SetupId && s.UserId == userId);

            if (setup == null) throw new KeyNotFoundException("Setup not found");

            var orders = await _context.ExchangeOrders
                .Include(o => o.Fills)
                .Where(o => request.OrderIds.Contains(o.Id) && o.UserId == userId)
                .ToListAsync();

            // 2. Створюємо або оновлюємо Execution
            var execution = setup.Execution ?? new Execution { SetupId = setup.Id };

            // Очищуємо старі зв'язки перед новими (якщо вони були)
            execution.Orders.Clear();
            foreach (var order in orders)
            {
                execution.Orders.Add(order);
            }

            // 3. РОЗРАХУНОК МАТЕМАТИКИ
            CalculateExecutionStats(execution, orders, setup);

            if (setup.Execution == null)
            {
                _context.Executions.Add(execution);
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<ExecutionResponseDto>(execution);
        }

        public async Task<List<ExchangeOrderResponseDto>> GetUnlinkedOrdersAsync(string userId, string symbol)
        {
            // Шукаємо ордери, які:
            // 1. Належать юзеру
            // 2. Потрібного символу (наприклад, BTCUSDT)
            // 3. Ще не мають прив'язаного ExecutionId
            var orders = await _context.ExchangeOrders
                .Include(o => o.Fills)
                .Where(o => o.UserId == userId &&
                            o.Symbol == symbol &&
                            o.ExecutionId == null)
                .OrderByDescending(o => o.OrderTime)
                .ToListAsync();

            return _mapper.Map<List<ExchangeOrderResponseDto>>(orders);
        }

        public async Task<ExecutionResponseDto?> GetBySetupIdAsync(Guid setupId, string userId)
        {
            var execution = await _context.Executions
                .Include(e => e.Orders)
                    .ThenInclude(o => o.Fills)
                .FirstOrDefaultAsync(e => e.SetupId == setupId && e.Setup.UserId == userId);

            return execution == null ? null : _mapper.Map<ExecutionResponseDto>(execution);
        }
        private void CalculateExecutionStats(Execution execution, List<ExchangeOrder> orders, Setup setup)
        {
            var allFills = orders.SelectMany(o => o.Fills).ToList();
            if (!allFills.Any()) return;

            // Розділяємо входи та виходи (Side залежить від напрямку сетапу)
            // Якщо Long: Buy = Entry, Sell = Exit. Якщо Short: Sell = Entry, Buy = Exit.
            var entryFills = allFills.Where(f => f.ExchangeOrder.Side.ToLower() == (setup.Direction == TradeDirection.Long ? "buy" : "sell")).ToList();
            var exitFills = allFills.Where(f => f.ExchangeOrder.Side.ToLower() == (setup.Direction == TradeDirection.Long ? "sell" : "buy")).ToList();

            // Середньозважені ціни
            decimal entryQty = entryFills.Sum(f => f.Qty);
            decimal exitQty = exitFills.Sum(f => f.Qty);

            execution.AvgEntryPrice = entryQty > 0 ? entryFills.Sum(f => f.Price * f.Qty) / entryQty : 0;
            execution.AvgExitPrice = exitQty > 0 ? exitFills.Sum(f => f.Price * f.Qty) / exitQty : 0;

            // Фінанси
            execution.TotalCommission = allFills.Sum(f => f.Commission);

            // PnL (Спрощено: (Exit - Entry) * Qty для Long)
            decimal multiplier = setup.Direction == TradeDirection.Long ? 1 : -1;
            execution.RealizedPnL = (execution.AvgExitPrice - execution.AvgEntryPrice) * exitQty * multiplier;
            execution.NetPnL = execution.RealizedPnL - execution.TotalCommission; // + Funding пізніше

            // ROI (PnL / Задіяний об'єм входу)
            decimal entryValue = execution.AvgEntryPrice * entryQty;
            execution.ROI = entryValue > 0 ? (execution.NetPnL / entryValue) * 100 : 0;

            // Slippage (Порівняння з планом із Setup)
            execution.SlippagePoints = execution.AvgEntryPrice - setup.EntryPrice;
            if (setup.EntryPrice != 0)
                execution.SlippagePercent = (execution.SlippagePoints / setup.EntryPrice) * 100;

            // Таймінг
            execution.OpenTime = allFills.Min(f => f.TradeTime);
            execution.CloseTime = exitQty >= entryQty ? allFills.Max(f => f.TradeTime) : null;
        }
    }
}

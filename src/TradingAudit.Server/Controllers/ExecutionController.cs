using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.DTOs.Executions;

namespace TradingAudit.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExecutionController : ControllerBase
{
    private readonly IExecutionService _executionService;

    public ExecutionController(IExecutionService executionService)
    {
        _executionService = executionService;
    }

    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [HttpGet("setup/{setupId}")]
    public async Task<ActionResult<ExecutionResponseDto>> GetBySetupId(Guid setupId)
    {
        var result = await _executionService.GetBySetupIdAsync(setupId, UserId);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("unlinked/{symbol}")]
    public async Task<ActionResult<List<ExchangeOrderResponseDto>>> GetUnlinkedOrders(string symbol)
    {
        // Декодуємо символ (наприклад, BTC/USDT -> BTCUSDT), якщо потрібно
        var sanitizedSymbol = symbol.Replace("/", "").ToUpper();
        var result = await _executionService.GetUnlinkedOrdersAsync(UserId, sanitizedSymbol);

        return Ok(result);
    }

    [HttpPost("link")]
    public async Task<ActionResult<ExecutionResponseDto>> LinkOrders([FromBody] LinkOrdersRequestDto request)
    {
        try
        {
            var result = await _executionService.LinkOrdersToSetupAsync(UserId, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Помилка при прив'язці ордерів: {ex.Message}");
        }
    }
}
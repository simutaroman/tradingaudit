using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.DTOs.Strategies;
using TradingAudit.Shared.Constants; // <--- Не забудь додати цей using
using System.Security.Claims;

namespace TradingAudit.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StrategiesController : ControllerBase
{
    private readonly IStrategyService _strategyService;
    private readonly UserManager<ApplicationUser> _userManager;

    public StrategiesController(IStrategyService strategyService, UserManager<ApplicationUser> userManager)
    {
        _strategyService = strategyService;
        _userManager = userManager;
    }

    // GET: Читання доступне всім авторизованим (User, Admin, Support)
    // Кожен бачить СВОЇ стратегії.
    // (Для перегляду стратегій інших юзерів адміном потрібен окремий AdminController)
    [HttpGet]
    public async Task<ActionResult<List<TradingStrategyDto>>> GetActive()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _strategyService.GetActiveStrategiesAsync(userId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TradingStrategyDto>> GetById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var strategy = await _strategyService.GetByIdAsync(userId, id);

        if (strategy == null) return NotFound();
        return strategy;
    }

    // POST: Створення вимагає права на ЗАПИС
    // Це відсікає роль Support (якщо ми їм вимкнули WriteAccess)
    [HttpPost]
    [Authorize(Policy = PolicyNames.RequireWriteAccess)]
    public async Task<IActionResult> Create(TradingStrategyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Unauthorized();

        try
        {
            // Передаємо tier для перевірки лімітів (1 стратегія для Free)
            var newId = await _strategyService.CreateAsync(userId, user.SubscriptionTier, dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: Редагування вимагає права на ЗАПИС
    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.RequireWriteAccess)]
    public async Task<IActionResult> Update(Guid id, TradingStrategyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var newVersionId = await _strategyService.UpdateAsync(userId, id, dto);
            return Ok(new { newVersionId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // DELETE: Видалення вимагає права на ЗАПИС
    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.RequireWriteAccess)]
    public async Task<IActionResult> Archive(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _strategyService.DeleteAsync(userId, id);
        return NoContent();
    }

    [HttpGet("history/{groupId}")]
    public async Task<ActionResult<List<TradingStrategyDto>>> GetHistory(Guid groupId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _strategyService.GetHistoryAsync(userId, groupId);
    }
}
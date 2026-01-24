using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingAudit.Server.Services;
using TradingAudit.Shared.DTOs.ExchangeKeys;

namespace TradingAudit.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExchangeSettingsController : ControllerBase
{
    private readonly IExchangeKeyService _keyService;
    private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

    public ExchangeSettingsController(IExchangeKeyService keyService)
    {
        _keyService = keyService;
    }

    [HttpGet("keys")]
    public async Task<ActionResult<List<ExchangeKeyResponseDto>>> GetKeys()
    {
        return Ok(await _keyService.GetUserKeysAsync(UserId));
    }

    [HttpPost("keys")]
    public async Task<IActionResult> AddKey([FromBody] ExchangeKeyRequestDto dto)
    {
        var result = await _keyService.AddKeyAsync(UserId, dto);
        return result ? Ok() : BadRequest("Не вдалося зберегти ключі");
    }

    [HttpDelete("keys/{id}")]
    public async Task<IActionResult> DeleteKey(Guid id)
    {
        var result = await _keyService.DeleteKeyAsync(id, UserId);
        return result ? Ok() : NotFound();
    }
}

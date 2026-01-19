using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradingAudit.Server.Services;
using TradingAudit.Server.Services.Interfaces;
using TradingAudit.Shared.Constants;
using TradingAudit.Shared.DTOs.Setups;

namespace TradingAudit.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SetupsController : ControllerBase
{
    private readonly ISetupService _setupService;

    public SetupsController(ISetupService setupService)
    {
        _setupService = setupService;
    }

    [HttpGet]
    // Політика ReadAccess поки не використовується, достатньо [Authorize]
    public async Task<ActionResult<List<SetupResponseDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        return Ok(await _setupService.GetAllAsync(userId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SetupResponseDto>> GetById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var result = await _setupService.GetByIdAsync(userId, id);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.RequireWriteAccess)]
    public async Task<ActionResult<SetupResponseDto>> Create(SetupCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var result = await _setupService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.RequireWriteAccess)]
    public async Task<ActionResult<SetupResponseDto>> Update(Guid id, SetupResponseDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var result = await _setupService.UpdateAsync(userId, id, dto);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.RequireWriteAccess)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _setupService.DeleteAsync(userId, id);
        return NoContent();
    }
}
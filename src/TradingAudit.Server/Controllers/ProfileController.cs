using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradingAudit.Server.Entities; // Твій ApplicationUser
using TradingAudit.Shared;
using System.Security.Claims;

namespace TradingAudit.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Отримати дані профілю
    [HttpGet]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null) return NotFound();

        return new UserProfileDto
        {
            Email = user.Email!,
            Nickname = user.Nickname,
            IsEmailConfirmed = user.EmailConfirmed
        };
    }

    // Оновити дані (Нікнейм)
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null) return NotFound();

        // Оновлюємо нікнейм
        user.Nickname = model.Nickname;

        // Тут можна додати логіку зміни Email, але це складніше (треба нове підтвердження).
        // Поки що дозволимо міняти тільки нік.

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }
}

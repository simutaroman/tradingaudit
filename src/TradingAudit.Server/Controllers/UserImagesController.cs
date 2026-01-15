using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Server.Services;
using TradingAudit.Shared;
using TradingAudit.Shared.Constants;

namespace TradingAudit.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserImagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBlobService _blobService;

    public UserImagesController(ApplicationDbContext context, IBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserImageDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return await _context.UserImages
            .Where(x => x.UserId == userId)
            .Select(x => new UserImageDto
            {
                Id = x.Id,
                Url = x.Url,
                Name = x.Name
            })
            .ToListAsync();
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.CanUploadImages)]
    public async Task<ActionResult<UserImageDto>> Upload([FromForm] string name, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("File is empty");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Завантажуємо в Azure Blob Storage
        using var stream = file.OpenReadStream();
        var fileUrl = await _blobService.UploadAsync(stream, file.FileName, file.ContentType);

        // 2. Зберігаємо запис в БД
        var userImage = new UserImage
        {
            UserId = userId,
            Name = name,
            Url = fileUrl // Тепер тут посилання на azurewebsites...
        };

        _context.UserImages.Add(userImage);
        await _context.SaveChangesAsync();

        return Ok(new UserImageDto { Id = userImage.Id, Name = userImage.Name, Url = userImage.Url });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.CanUploadImages)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var image = await _context.UserImages.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (image == null) return NotFound();

        await _blobService.DeleteAsync(image.Url);

        _context.UserImages.Remove(image);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
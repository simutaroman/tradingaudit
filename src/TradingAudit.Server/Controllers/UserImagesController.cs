using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Data;
using TradingAudit.Server.Entities;
using TradingAudit.Shared;

namespace TradingAudit.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Тільки для авторизованих
public class UserImagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    // Primary Constructor
    public UserImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // GET: api/UserImages
    [HttpGet]
    public async Task<ActionResult<List<UserImageDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Вибираємо тільки картинки поточного користувача
        var images = await _context.UserImages
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new UserImageDto
            {
                Id = x.Id,
                Name = x.Name,
                Url = x.Url
            })
            .ToListAsync();

        return Ok(images);
    }

    // POST: api/UserImages
    [HttpPost]
    public async Task<ActionResult<UserImageDto>> Upload([FromForm] string name, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не обрано.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        // 1. Генеруємо унікальне ім'я файлу
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";

        // 2. Визначаємо шлях збереження (wwwroot/uploads)
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);

        // 3. Зберігаємо файл фізично
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 4. Формуємо URL (відносний шлях)
        // Для Azure тут буде повний URL блоба
        var fileUrl = $"/uploads/{fileName}";

        // 5. Зберігаємо запис в БД
        var entity = new UserImage
        {
            Id = Guid.NewGuid(),
            Name = name,
            Url = fileUrl,
            UserId = userId
        };

        _context.UserImages.Add(entity);
        await _context.SaveChangesAsync();

        // 6. Повертаємо DTO
        var dto = new UserImageDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Url = entity.Url
        };

        return Ok(dto);
    }

    // DELETE: api/UserImages/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Шукаємо картинку, яка належить саме цьому юзеру
        var entity = await _context.UserImages
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (entity == null)
            return NotFound();

        // 1. Видаляємо файл фізично (якщо це локальний файл)
        // Увага: цей код специфічний для локального зберігання
        try
        {
            var fileName = Path.GetFileName(entity.Url);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            // Логування помилки видалення файлу, але запис з БД все одно видаляємо
            Console.WriteLine($"Error deleting file: {ex.Message}");
        }

        // 2. Видаляємо з БД
        _context.UserImages.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
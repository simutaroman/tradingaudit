using Microsoft.AspNetCore.Identity; // Тільки для типу IdentityUser, якщо потрібна навігація

namespace TradingAudit.Server.Entities;

public class UserImage
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    // Зовнішній ключ
    public string UserId { get; set; } = string.Empty;

    // Навігаційна властивість (опціонально, але зручно для EF)
    public virtual IdentityUser? User { get; set; }
}
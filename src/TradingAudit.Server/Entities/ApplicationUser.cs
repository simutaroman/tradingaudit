using Microsoft.AspNetCore.Identity;

namespace TradingAudit.Server.Entities;

public class ApplicationUser : IdentityUser
{
    public string? Nickname { get; set; }
}

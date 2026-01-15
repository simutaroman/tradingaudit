using Microsoft.AspNetCore.Identity;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Entities;

public class ApplicationUser : IdentityUser
{
    public string? Nickname { get; set; }
    
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;

    public DateTime? SubscriptionExpiresAt { get; set; }
}

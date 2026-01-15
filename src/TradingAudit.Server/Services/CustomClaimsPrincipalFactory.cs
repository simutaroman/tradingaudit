using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Services;

// Цей клас розширює стандартну фабрику створення "паспорта" користувача
public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public CustomClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // 1. Створюємо стандартні клейми (id, email, roles)
        var identity = await base.GenerateClaimsAsync(user);

        // 2. Додаємо наш кастомний клейм - Рівень підписки
        // Зберігаємо як рядок (наприклад "1" для Standard)
        identity.AddClaim(new Claim("subscription_tier", ((int)user.SubscriptionTier).ToString()));

        return identity;
    }
}
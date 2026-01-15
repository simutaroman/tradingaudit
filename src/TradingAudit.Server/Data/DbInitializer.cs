using Microsoft.AspNetCore.Identity;
using TradingAudit.Server.Entities;
using TradingAudit.Shared.Enums;

namespace TradingAudit.Server.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 1. Створюємо ролі, якщо їх немає
        string[] roleNames = { "Admin", "Support" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Створюємо Супер-Адміна (ТЕБЕ)
        // Зміни email та пароль на свої!
        var adminEmail = "simutaroman@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nickname = "SuperAdmin",
                EmailConfirmed = true, // Одразу підтверджений
                SubscriptionTier = SubscriptionTier.Pro // Максимальний рівень
            };

            // Пароль має бути складним (Identity вимагає: цифри, великі літери, спецсимволи)
            var result = await userManager.CreateAsync(adminUser, "Inspiron_2026");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}

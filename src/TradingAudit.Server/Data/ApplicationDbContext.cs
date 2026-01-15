using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserImage> UserImages { get; set; }

    public DbSet<TradingStrategy> TradingStrategies { get; set; }
    public DbSet<StrategyRule> StrategyRules { get; set; }
    // DbSet<StrategyImage> та StrategyRuleImage не обов'язкові тут, 
    // бо вони доступні через навігаційні властивості, але можна додати для прямих запитів.
    public DbSet<StrategyImage> StrategyImages { get; set; }
    public DbSet<StrategyRuleImage> StrategyRuleImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // !! ВАЖЛИВО: Спочатку викликаємо базовий метод для налаштування Identity таблиць
        base.OnModelCreating(builder);

        // Автоматично застосовує всі класи, що реалізують IEntityTypeConfiguration в цій збірці
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
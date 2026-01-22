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

    // todo: delete user images
    public DbSet<UserImage> UserImages { get; set; }

    public DbSet<TradingStrategy> TradingStrategies { get; set; }
    public DbSet<StrategyRule> StrategyRules { get; set; }
    public DbSet<StrategyImage> StrategyImages { get; set; }
    public DbSet<StrategyRuleImage> StrategyRuleImages { get; set; }
    public DbSet<Setup> Setups { get; set; }
    public DbSet<SetupChecklistItem> SetupChecklistItems { get; set; }
    public DbSet<SetupImage> SetupImages { get; set; }
    public DbSet<SetupChecklistItemImage> SetupChecklistItemImages { get; set; }
    public DbSet<Execution> Executions => Set<Execution>();
    public DbSet<ExchangeOrder> ExchangeOrders => Set<ExchangeOrder>();
    public DbSet<OrderFill> OrderFills => Set<OrderFill>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
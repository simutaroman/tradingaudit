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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // !! ВАЖЛИВО: Спочатку викликаємо базовий метод для налаштування Identity таблиць
        base.OnModelCreating(builder);

        // Автоматично застосовує всі класи, що реалізують IEntityTypeConfiguration в цій збірці
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
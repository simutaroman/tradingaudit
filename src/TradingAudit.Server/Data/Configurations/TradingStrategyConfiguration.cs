using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class TradingStrategyConfiguration : IEntityTypeConfiguration<TradingStrategy>
{
    public void Configure(EntityTypeBuilder<TradingStrategy> builder)
    {
        builder.ToTable("TradingStrategies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000); // Опис може бути довгим

        // Scope поля (зберігаємо як CSV string)
        builder.Property(x => x.Timeframes).HasMaxLength(200);
        builder.Property(x => x.AssetScope).HasMaxLength(200);

        // Налаштування для Decimal (гроші та R:R)
        builder.Property(x => x.DefaultRiskAmount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinRiskRewardRatio).HasColumnType("decimal(18,2)");

        // Індекси для швидкодії
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.GroupId);
        // Унікальність версії в межах однієї групи
        builder.HasIndex(x => new { x.GroupId, x.Version }).IsUnique();

        // Зв'язки
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Видалили юзера -> видалили стратегії

        builder.HasMany(x => x.Rules)
            .WithOne(r => r.Strategy)
            .HasForeignKey(r => r.StrategyId)
            .OnDelete(DeleteBehavior.Cascade); // Видалили стратегію -> видалили правила

        builder.HasMany(x => x.Images)
            .WithOne(i => i.Strategy)
            .HasForeignKey(i => i.StrategyId)
            .OnDelete(DeleteBehavior.Cascade); // Видалили стратегію -> видалили картинки
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class SetupConfiguration : IEntityTypeConfiguration<Setup>
{
    public void Configure(EntityTypeBuilder<Setup> builder)
    {
        builder.HasKey(x => x.Id);

        // Основні поля
        builder.Property(x => x.Symbol).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Timeframe).IsRequired().HasMaxLength(10);
        builder.Property(x => x.TradingThesis).HasMaxLength(2000);
        builder.Property(x => x.Mood).HasMaxLength(500); // string для тегів/тексту

        // Точність для цін та об'ємів (важливо для крипти)
        builder.Property(x => x.EntryPrice).HasPrecision(18, 8);
        builder.Property(x => x.StopLoss).HasPrecision(18, 8);
        builder.Property(x => x.TakeProfit).HasPrecision(18, 8);
        builder.Property(x => x.PositionSize).HasPrecision(18, 8);

        // Зв'язки
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Strategy)
            .WithMany() // У стратегії немає прямого списку сетапів (для чистоти)
            .HasForeignKey(x => x.StrategyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Setups");
    }
}
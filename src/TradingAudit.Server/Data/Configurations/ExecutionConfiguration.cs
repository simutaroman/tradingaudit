using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

public class ExecutionConfiguration : IEntityTypeConfiguration<Execution>
{
    public void Configure(EntityTypeBuilder<Execution> builder)
    {
        builder.ToTable("Executions");
        builder.HasKey(x => x.Id);

        // Налаштування для Decimal (точність для крипти)
        builder.Property(x => x.RealizedPnL).HasColumnType("decimal(18,8)");
        builder.Property(x => x.NetPnL).HasColumnType("decimal(18,8)");
        builder.Property(x => x.TotalCommission).HasColumnType("decimal(18,8)");
        builder.Property(x => x.TotalFunding).HasColumnType("decimal(18,8)");
        builder.Property(x => x.AvgEntryPrice).HasColumnType("decimal(18,8)");
        builder.Property(x => x.AvgExitPrice).HasColumnType("decimal(18,8)");
        builder.Property(x => x.ROI).HasColumnType("decimal(18,4)"); // Для % достатньо 4 знаки
        builder.Property(x => x.SlippagePoints).HasColumnType("decimal(18,8)");
        builder.Property(x => x.SlippagePercent).HasColumnType("decimal(18,4)");

        // Зв'язки
        builder.HasOne(x => x.Setup)
            .WithOne(s => s.Execution)
            .HasForeignKey<Execution>(x => x.SetupId)
            .OnDelete(DeleteBehavior.Cascade); // Видалили сетап -> видалили виконання
    }
}
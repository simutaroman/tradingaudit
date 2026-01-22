using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

public class OrderFillConfiguration : IEntityTypeConfiguration<OrderFill>
{
    public void Configure(EntityTypeBuilder<OrderFill> builder)
    {
        builder.ToTable("OrderFills");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalFillId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CommissionAsset).HasMaxLength(10).HasDefaultValue("USDT");

        builder.Property(x => x.Price).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Qty).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Commission).HasColumnType("decimal(18,8)");

        // Унікальність трейду
        builder.HasIndex(x => x.ExternalFillId).IsUnique();

        // Зв'язки
        // ВИПРАВЛЕНО: Використовуємо DeleteBehavior.Restrict, щоб уникнути Multiple Cascade Paths
        builder.HasOne(x => x.ExchangeOrder)
            .WithMany(o => o.Fills)
            .HasForeignKey(x => x.ExchangeOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
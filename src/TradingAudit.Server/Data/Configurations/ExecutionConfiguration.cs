using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

public class ExchangeOrderConfiguration : IEntityTypeConfiguration<ExchangeOrder>
{
    public void Configure(EntityTypeBuilder<ExchangeOrder> builder)
    {
        builder.ToTable("ExchangeOrders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalOrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Symbol).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Side).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Type).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(20);

        builder.Property(x => x.Price).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Amount).HasColumnType("decimal(18,8)");
        builder.Property(x => x.ExecutedAmount).HasColumnType("decimal(18,8)");

        // Індекси
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.ExternalOrderId, x.UserId }).IsUnique();

        // Зв'язки
        builder.HasOne(x => x.Execution)
            .WithMany(e => e.Orders)
            .HasForeignKey(x => x.ExecutionId)
            .OnDelete(DeleteBehavior.SetNull); // Видалили аналітику -> ордери стали "сиротами", але не зникли
    }
}
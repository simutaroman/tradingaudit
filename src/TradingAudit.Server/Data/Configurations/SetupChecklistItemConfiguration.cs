using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class SetupChecklistItemConfiguration : IEntityTypeConfiguration<SetupChecklistItem>
{
    public void Configure(EntityTypeBuilder<SetupChecklistItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Comment).HasMaxLength(1000);
        builder.Property(x => x.OrderIndex).IsRequired();

        // Каскадне видалення: видалив сетап — видалився чек-лист
        builder.HasOne(x => x.Setup)
            .WithMany(s => s.ChecklistItems)
            .HasForeignKey(x => x.SetupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Посилання на правило стратегії (заборона видалення правила, якщо є сетап)
        builder.HasOne(x => x.StrategyRule)
            .WithMany()
            .HasForeignKey(x => x.StrategyRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("SetupChecklistItems");
    }
}
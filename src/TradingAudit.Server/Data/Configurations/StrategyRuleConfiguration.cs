using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class StrategyRuleConfiguration : IEntityTypeConfiguration<StrategyRule>
{
    public void Configure(EntityTypeBuilder<StrategyRule> builder)
    {
        builder.ToTable("StrategyRules");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.Property(x => x.LogicParameter).HasColumnType("decimal(18,2)");

        // Зв'язок з картинками правила
        builder.HasMany(x => x.Images)
            .WithOne(i => i.StrategyRule)
            .HasForeignKey(i => i.StrategyRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
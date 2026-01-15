using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class StrategyRuleImageConfiguration : IEntityTypeConfiguration<StrategyRuleImage>
{
    public void Configure(EntityTypeBuilder<StrategyRuleImage> builder)
    {
        builder.ToTable("StrategyRuleImages");
        builder.Property(x => x.Url).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}

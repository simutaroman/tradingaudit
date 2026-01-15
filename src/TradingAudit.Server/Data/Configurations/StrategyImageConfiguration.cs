using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class StrategyImageConfiguration : IEntityTypeConfiguration<StrategyImage>
{
    public void Configure(EntityTypeBuilder<StrategyImage> builder)
    {
        builder.ToTable("StrategyImages");
        builder.Property(x => x.Url).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}

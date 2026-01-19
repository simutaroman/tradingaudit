using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class SetupImageConfiguration : IEntityTypeConfiguration<SetupImage>
{
    public void Configure(EntityTypeBuilder<SetupImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasOne(x => x.Setup)
            .WithMany(s => s.Images)
            .HasForeignKey(x => x.SetupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("SetupImages");
    }
}

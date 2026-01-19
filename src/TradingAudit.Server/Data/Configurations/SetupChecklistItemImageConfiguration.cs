using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class SetupChecklistItemImageConfiguration : IEntityTypeConfiguration<SetupChecklistItemImage>
{
    public void Configure(EntityTypeBuilder<SetupChecklistItemImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).IsRequired();

        builder.HasOne(x => x.SetupChecklistItem)
            .WithMany(i => i.Images)
            .HasForeignKey(x => x.SetupChecklistItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("SetupChecklistItemImages");
    }
}

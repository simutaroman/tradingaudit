using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class UserExchangeKeyConfiguration : IEntityTypeConfiguration<UserExchangeKey>
{
    public void Configure(EntityTypeBuilder<UserExchangeKey> builder)
    {
        builder.ToTable("UserExchangeKeys");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.ExchangeName).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ApiKey).IsRequired().HasMaxLength(500); // Довше через шифрування
        builder.Property(x => x.ApiSecret).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Label).HasMaxLength(100);

        // Унікальність: один юзер не може додати той самий ключ для тієї ж біржі двічі
        builder.HasIndex(x => new { x.UserId, x.ExchangeName, x.ApiKey }).IsUnique();

        // Індекс для швидкої вибірки всіх ключів юзера
        builder.HasIndex(x => x.UserId);
    }
}

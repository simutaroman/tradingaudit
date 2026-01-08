using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingAudit.Server.Entities;

namespace TradingAudit.Server.Data.Configurations;

public class UserImageConfiguration : IEntityTypeConfiguration<UserImage>
{
    public void Configure(EntityTypeBuilder<UserImage> builder)
    {
        // Первинний ключ
        builder.HasKey(x => x.Id);

        // Налаштування полів
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Url)
            .IsRequired();

        // Налаштування зв'язку з User
        builder.HasOne(x => x.User)
            .WithMany() // У IdentityUser немає колекції Images, тому тут пусто
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // Якщо видалити юзера - видаляться і картинки
    }
}
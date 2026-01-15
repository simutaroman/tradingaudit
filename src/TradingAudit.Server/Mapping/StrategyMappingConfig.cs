using Mapster;
using TradingAudit.Server.Entities;
using TradingAudit.Shared.DTOs.Strategies;

namespace TradingAudit.Server.Mapping;

public class StrategyMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // === Strategy DTO -> Entity ===
        config.NewConfig<TradingStrategyDto, TradingStrategy>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.User)
            .Ignore(dest => dest.UserId)
            // Явно вказуємо перетворення списку рядків у колекцію об'єктів
            .Map(dest => dest.Images, src => src.ImageUrls.Select(url => new StrategyImage { Url = url }));

        // === Entity -> Strategy DTO ===
        config.NewConfig<TradingStrategy, TradingStrategyDto>()
            .Map(dest => dest.ImageUrls, src => src.Images.Select(img => img.Url));

        // === Rule DTO -> Entity ===
        config.NewConfig<StrategyRuleDto, StrategyRule>()
            .IgnoreIf((src, dest) => src.Id == null, dest => dest.Id)
            .Map(dest => dest.Images, src => src.ImageUrls.Select(url => new StrategyRuleImage { Url = url }));

        // === Entity -> Rule DTO ===
        config.NewConfig<StrategyRule, StrategyRuleDto>()
            .Map(dest => dest.ImageUrls, src => src.Images.Select(img => img.Url));
    }
}
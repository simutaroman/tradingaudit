//using Mapster;
//using TradingAudit.Server.Entities;
//using TradingAudit.Shared.DTOs.Strategies;

//namespace TradingAudit.Server.Mapping;

//public static class MapsterConfig
//{
//    public static void Configure()
//    {
//        // === Strategy DTO -> Entity ===
//        TypeAdapterConfig<TradingStrategyDto, TradingStrategy>
//            .NewConfig()
//            .Ignore(dest => dest.Id)      // ID генеруємо самі або база
//            .Ignore(dest => dest.User)    // Юзера проставляємо вручну
//            .Ignore(dest => dest.UserId)
//            //.Ignore(dest => dest.CreatedBy) // Якщо є таке поле
//                                            // Мапінг списку URL-ів у колекцію об'єктів StrategyImage
//            .Map(dest => dest.Images, src => src.ImageUrls.Select(url => new StrategyImage { Url = url }));

//        // === Entity -> Strategy DTO ===
//        TypeAdapterConfig<TradingStrategy, TradingStrategyDto>
//            .NewConfig()
//            // Мапінг колекції об'єктів у список URL-ів
//            .Map(dest => dest.ImageUrls, src => src.Images.Select(img => img.Url));


//        // === Rule DTO -> Entity ===
//        TypeAdapterConfig<StrategyRuleDto, StrategyRule>
//            .NewConfig()
//            // Якщо ID прийшов null (нове правило), ігноруємо його, EF згенерує
//            .IgnoreIf((src, dest) => src.Id == null, dest => dest.Id)
//            .Map(dest => dest.Images, src => src.ImageUrls.Select(url => new StrategyRuleImage { Url = url }));

//        // === Entity -> Rule DTO ===
//        TypeAdapterConfig<StrategyRule, StrategyRuleDto>
//            .NewConfig()
//            .Map(dest => dest.ImageUrls, src => src.Images.Select(img => img.Url));
//    }
//}
using Mapster;
using TradingAudit.Server.Entities;
using TradingAudit.Shared.DTOs.Setups;

namespace TradingAudit.Server.Mapping;

public class SetupMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // --- Поточна конфігурація Entity -> DTO (Read) ---
        config.NewConfig<Setup, SetupResponseDto>()
            .Map(dest => dest.StrategyName, src => src.Strategy != null ? src.Strategy.Name : string.Empty)
            .Map(dest => dest.StrategyVersion, src => src.Strategy != null ? src.Strategy.Version : 0)
            .Map(dest => dest.Images, src => src.Images)
            .Map(dest => dest.Checklist, src => src.ChecklistItems);

        config.NewConfig<SetupChecklistItem, SetupChecklistItemDto>()
            .Map(dest => dest.RuleTitle, src => src.StrategyRule != null ? src.StrategyRule.Title : "Deleted Rule")
            .Map(dest => dest.RuleDescription, src => src.StrategyRule != null ? src.StrategyRule.Description : string.Empty)
            .Map(dest => dest.IsMandatory, src => src.StrategyRule != null ? src.StrategyRule.IsMandatory : false)
            .Map(dest => dest.Images, src => src.Images);

        // --- НОВА КОНФІГУРАЦІЯ DTO -> Entity (Write/Update) ---

        // Мапимо ResponseDto назад у Setup (Entity)
        config.NewConfig<SetupResponseDto, Setup>()
            .Map(dest => dest.ChecklistItems, src => src.Checklist) // Тут ми кажемо маперу куди класти дані з Checklist
            .Map(dest => dest.Images, src => src.Images)
            .Ignore(dest => dest.Strategy)     // Не чіпаємо навігаційні властивості стратегії
            .Ignore(dest => dest.UserId);

        // Мапимо ChecklistItemDto назад у сутність
        config.NewConfig<SetupChecklistItemDto, SetupChecklistItem>()
            .Map(dest => dest.Images, src => src.Images)
            .Ignore(dest => dest.StrategyRule); // Не даємо маперу створити дублікат правила стратегії

        // Решта (Картинки)
        config.NewConfig<SetupImageDto, SetupImage>();
        config.NewConfig<SetupImageDto, SetupChecklistItemImage>();
    }
}
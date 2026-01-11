using System.ComponentModel.DataAnnotations;

namespace TradingAudit.Shared;

public class UserProfileDto
{
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20, ErrorMessage = "Нікнейм занадто довгий (макс 20 символів)")]
    public string? Nickname { get; set; }

    public bool IsEmailConfirmed { get; set; }
}

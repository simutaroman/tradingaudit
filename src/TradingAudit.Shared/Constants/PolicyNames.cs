namespace TradingAudit.Shared.Constants;

public static class PolicyNames
{
    // Політики функціоналу
    public const string CanUploadImages = "CanUploadImages";   // Standard+ або Admin
    public const string CanExportData = "CanExportData";       // Pro+ або Admin або Support

    // Політики доступу
    public const string RequireAdmin = "RequireAdmin";         // Тільки Admin
    public const string RequireWriteAccess = "RequireWriteAccess"; // Admin або User (не Support)
}
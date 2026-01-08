using Microsoft.AspNetCore.Components.Forms;
using TradingAudit.Shared;

namespace TradingAudit.Client.Services;

public interface IImageService
{
    Task<List<UserImageDto>> GetAllImages();
    Task<UserImageDto?> UploadImage(IBrowserFile file, string name);
    Task DeleteImage(Guid id);
}
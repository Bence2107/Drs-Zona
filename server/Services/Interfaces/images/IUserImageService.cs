using Microsoft.AspNetCore.Http;

namespace Services.Interfaces.images;

public interface IUserImageService
{
    string? GetAvatarUrl(Guid userId);
    Task<string> SaveAvatar(Guid userId, IFormFile file);
    void DeleteAvatar(Guid userId);
}
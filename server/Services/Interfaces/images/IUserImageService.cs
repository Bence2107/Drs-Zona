using Microsoft.AspNetCore.Http;

namespace Services.Interfaces.images;

public interface IUserImageService
{
    string? GetAvatarUrl(Guid userId);
    Task<ResponseResult<bool>> SaveAvatar(Guid userId, IFormFile? file);
    ResponseResult<bool> DeleteAvatar(Guid userId);
}
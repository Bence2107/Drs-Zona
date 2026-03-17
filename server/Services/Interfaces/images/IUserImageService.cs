using Microsoft.AspNetCore.Http;
using Services.Types;

namespace Services.Interfaces.images;

public interface IUserImageService
{
    Task<string?> GetAvatarUrl(Guid userId);
    Task<ResponseResult<bool>> SaveAvatar(Guid userId, IFormFile? file);
    Task<ResponseResult<bool>> DeleteAvatar(Guid userId);
}
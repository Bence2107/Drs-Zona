using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;

namespace Services.Implementations.image;

public class UserImageService(IWebHostEnvironment env) : IUserImageService
{
    public string? GetAvatarUrl(Guid userId)
    {
        var userFolder = Path.Combine(env.ContentRootPath, "uploads", "images", "avatars", userId.ToString());

        if (!Directory.Exists(userFolder))
        {
            return null; 
        }
        
        var existing = Directory.GetFiles(userFolder, "avatar.jpg").FirstOrDefault();
    
        return existing == null ? null : $"/uploads/images/avatars/{userId}/avatar.jpg";
    }

    public async Task<string> SaveAvatar(Guid userId, IFormFile file)
    {
        var userFolder = Path.Combine(env.ContentRootPath, "uploads", "images", "avatars", userId.ToString());
    
        if (!Directory.Exists(userFolder))
        {
            Directory.CreateDirectory(userFolder);
        }

        var filePath = Path.Combine(userFolder, "avatar.jpg");

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/images/avatars/{userId}/avatar.jpg";
    }

    public void DeleteAvatar(Guid userId)
    {
        var userFolder = Path.Combine(env.ContentRootPath, "uploads", "images", "avatars", userId.ToString());
    
        if (Directory.Exists(userFolder))
        {
            Directory.Delete(userFolder, true);
        }
    }
}
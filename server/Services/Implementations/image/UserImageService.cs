using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;

namespace Services.Implementations.image;

public class UserImageService(IWebHostEnvironment env) : IUserImageService
{
    public string? GetAvatarUrl(Guid userId)
    {
        var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "images", "avatars", userId.ToString());
        var existing = Directory.GetFiles(uploadsPath, "avatar.jpg").FirstOrDefault();
        return existing == null ? null : $"/uploads/images/avatars/{userId}/avatar.jpg";
    }

    public async Task<string> SaveAvatar(Guid userId, IFormFile file)
    {
        var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "images", "avatars");
        Directory.CreateDirectory(uploadsPath);

        var fileName = $"{userId}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/images/avatars/{fileName}";
    }

    public void DeleteAvatar(Guid userId)
    {
        var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "images", "avatars");
        var existing = Directory.GetFiles(uploadsPath, $"{userId}.*").FirstOrDefault();
        if (existing != null) File.Delete(existing);
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;

namespace Services.Implementations.image;

public class UserImageService(IWebHostEnvironment env) : IUserImageService
{
    private string GetUserFolderPath(Guid userId) => 
        Path.Combine(env.ContentRootPath, "uploads", "images", "avatars", userId.ToString());
    
    public string? GetAvatarUrl(Guid userId)
    {
        var userFolder = GetUserFolderPath(userId);

        if (!Directory.Exists(userFolder)) return null;
        
        var existing = Directory.GetFiles(userFolder, "avatar.jpg").FirstOrDefault();
        return existing == null ? null : $"/uploads/images/avatars/{userId}/avatar.jpg";
    } 

    public async Task<ResponseResult<bool>> SaveAvatar(Guid userId, IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return ResponseResult<bool>.Failure("File", "Üres fájl.");
        }
        
        const int maxFileSize = 2 * 1024 * 1024;
        if (file.Length > maxFileSize)
        {
            return ResponseResult<bool>.Failure("File", "A fájl mérete nem haladhatja meg a 2MB-ot.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            return ResponseResult<bool>.Failure("File", "Csak JPG, JPEG és PNG fájlok engedélyezettek.");
        }

        var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
        if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return ResponseResult<bool>.Failure("File", "Érvénytelen képformátum.");
        }
        
        try
        {
            var userFolder = GetUserFolderPath(userId);
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            var filePath = Path.Combine(userFolder, "avatar.jpg");
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return ResponseResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ResponseResult<bool>.Failure("Server", ex.Message);
        }
    }

    public ResponseResult<bool> DeleteAvatar(Guid userId)
    {
        try
        {
            var userFolder = GetUserFolderPath(userId);

            if (Directory.Exists(userFolder))
            {
                Directory.Delete(userFolder, true);
                return ResponseResult<bool>.Success(true);
            }

            return ResponseResult<bool>.Failure("Folder", "A Profilkép nem található");
        }
        catch (Exception)
        {
            return ResponseResult<bool>.Failure("Server", "Sikertelen művelet");
        }
    }
}
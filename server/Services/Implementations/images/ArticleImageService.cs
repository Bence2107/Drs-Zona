using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Services.Implementations.images;

public class ArticleImageService(IWebHostEnvironment env): IArticleImageService
{
    public string? GetImageUrl(string slug, string imageName)
    {
        var relativePath = $"uploads/images/articles/{slug}/{imageName}";
       
        var physicalPath = Path.Combine(env.ContentRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        return File.Exists(physicalPath) ? $"/{relativePath}" : null;
    }

    private static string GetDraftImageUrl(string draftId, string imageName)
    {
        return $"/uploads/dumps/articles/{draftId}/{imageName}";
    }
     
    public async Task<string> SaveDraftImage(string draftId, IFormFile file, string imageName)
    {
        var dumpsPath = Path.Combine(env.ContentRootPath, "uploads", "dumps", "articles", draftId);
        Directory.CreateDirectory(dumpsPath);

        foreach (var old in Directory.GetFiles(dumpsPath, $"{imageName}.*"))
        {
            File.Delete(old);
        }

        var filePath = Path.Combine(dumpsPath, $"{imageName}.jpg");

        try
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());
            await image.SaveAsync(filePath, new JpegEncoder { Quality = 85 });
        }
        catch (Exception ex)
        {
            throw new Exception("Hiba a képfeldolgozás során.", ex);
        }

        return GetDraftImageUrl(draftId, $"{imageName}.jpg");
    }

    public async Task DeleteDraftImages(string draftId)
    {
        var folderPath = Path.Combine(env.ContentRootPath, "uploads", "dumps", "articles", draftId);
        if (Directory.Exists(folderPath))
        {
            await Task.Run(() => Directory.Delete(folderPath, true));
        }
    }
    
    public async Task PromoteDraftImages(string draftId, string slug)
    {
        var draftFolder = Path.Combine(env.ContentRootPath, "uploads", "dumps", "articles", draftId);
        if (!Directory.Exists(draftFolder)) return;

        var publishFolder = Path.Combine(env.ContentRootPath, "uploads", "images", "articles", slug);
        Directory.CreateDirectory(publishFolder);

        foreach (var srcFile in Directory.GetFiles(draftFolder))
        {
            var fileName = Path.GetFileName(srcFile);
            var destFile = Path.Combine(publishFolder, fileName);

            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }
        
            File.Move(srcFile, destFile); 
        }

        await DeleteDraftImages(draftId);
    }
}
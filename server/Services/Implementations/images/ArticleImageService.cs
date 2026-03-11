using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;

namespace Services.Implementations.images;

public class ArticleImageService(IWebHostEnvironment env): IArticleImageService
{
    public string? GetImageUrl(string slug, string imageName)
    {
        var relativePath = $"uploads/images/articles/{slug}/{imageName}";
       
        var physicalPath = Path.Combine(env.ContentRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        return File.Exists(physicalPath) ? $"/{relativePath}" : null;
    }
    
    public async Task<string?> SaveImage(string slug, IFormFile file, string imageName)
    {
        var uploadsPath = Path.Combine(env.ContentRootPath, "uploads", "images", "articles", slug);
        Directory.CreateDirectory(uploadsPath);
        
        var fileName = $"{imageName}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        return GetImageUrl(slug, imageName);
    }
    
    public async Task DeleteArticleImages(string slug)
    {
        var folderPath = Path.Combine(env.ContentRootPath, "uploads", "images", "articles", slug);
        if (Directory.Exists(folderPath))
        {
            await Task.Run(() => Directory.Delete(folderPath, true));
        }
    }

    private string GetDraftImageUrl(string draftId, string imageName)
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

        var fileName = $"{imageName}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(dumpsPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return GetDraftImageUrl(draftId, fileName);
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
            var destFile = Path.Combine(publishFolder, Path.GetFileName(srcFile));
            File.Copy(srcFile, destFile, overwrite: true);
        }

        await DeleteDraftImages(draftId);
    }
}
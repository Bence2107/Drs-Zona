using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;

namespace Services.Implementations.image;

public class ArticleImageService(IWebHostEnvironment env): IArticleImageService
{
    public string GetImageUrl(string slug, string imageName)
    {
        return $"/uploads/images/articles/{slug}/{imageName}";
    }
    
    public async Task<string> SaveImage(string slug, IFormFile file, string imageName)
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
    
    public void DeleteArticleImages(string slug)
    {
        var folderPath = Path.Combine(env.ContentRootPath, "uploads", "images", "articles", slug);
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
        }
    }
}
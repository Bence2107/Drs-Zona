using Microsoft.AspNetCore.Http;

namespace Services.Interfaces.images;

public interface IArticleImageService
{
    string GetImageUrl(string slug, string imageName);
    Task<string> SaveImage(string slug, IFormFile file, string imageName);
    void DeleteArticleImages(string slug);

}
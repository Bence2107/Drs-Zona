using Microsoft.AspNetCore.Http;

namespace Services.Interfaces.images;

public interface IArticleImageService
{
    string? GetImageUrl(string slug, string imageName);
    Task<string?> SaveImage(string slug, IFormFile file, string imageName);
    Task DeleteArticleImages(string slug);
    Task<string> SaveDraftImage(string draftId, IFormFile file, string imageName);
    Task DeleteDraftImages(string draftId);
    Task PromoteDraftImages(string draftId, string slug);
}
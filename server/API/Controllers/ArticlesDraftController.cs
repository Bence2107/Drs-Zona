namespace Drs_Zona.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.images;

[ApiController]
[Route("api/article-image")]
[Authorize] 
public class ArticleImageController(IArticleImageService articleImageService) : ControllerBase 
{
    private static readonly string[] AllowedSlots = ["primary", "secondary", "third", "last"];
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; 
    
    [Authorize(Policy = "AuthorOrAdmin")]
    [HttpPost("{draftId}/images/{slot}")]
    public async Task<IActionResult> UploadDraftImage(string draftId, string slot, IFormFile? file)
    {
        if (!Guid.TryParse(draftId, out _))
            return BadRequest("A draftId érvénytelen GUID formátum.");

        if (!AllowedSlots.Contains(slot))
            return BadRequest($"Érvénytelen slot. Lehetséges értékek: {string.Join(", ", AllowedSlots)}");

        if (file == null || file.Length == 0)
            return BadRequest("Nem érkezett fájl.");

        if (file.Length > MaxFileSizeBytes)
            return BadRequest("A fájl mérete meghaladja az 5 MB-os limitet.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest($"Nem engedélyezett fájltípus. Lehetséges: {string.Join(", ", AllowedExtensions)}");

        var url = await articleImageService.SaveDraftImage(draftId, file, slot);
        return Ok(new { url });
    }
    
    [Authorize(Policy = "AuthorOrAdmin")]
    [HttpDelete("{draftId}")]
    public async Task<IActionResult> DeleteDraft(string draftId)
    {
        if (!Guid.TryParse(draftId, out _))
            return BadRequest("A draftId érvénytelen GUID formátum.");

        await articleImageService.DeleteDraftImages(draftId);
        return NoContent();
    }
    
    [Authorize(Policy = "AuthorOrAdmin")]
    [HttpPost("{draftId}/promote/{slug}")]
    public async Task<IActionResult> PromoteDraft(string draftId, string slug)
    {
        if (!Guid.TryParse(draftId, out _))
            return BadRequest("A draftId érvénytelen GUID formátum.");

        await articleImageService.PromoteDraftImages(draftId, slug);
        return NoContent();
    }
}
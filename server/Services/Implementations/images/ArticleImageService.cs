using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Interfaces.images;
using SkiaSharp;

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
            await using var input = file.OpenReadStream();
            using var codec = SKCodec.Create(input) ?? throw new InvalidOperationException("Unsupported image format.");
            using var decoded = SKBitmap.Decode(codec) ?? throw new InvalidOperationException("Image could not be decoded.");
            using var oriented = codec.EncodedOrigin == SKEncodedOrigin.TopLeft ? null : ApplyOrientation(decoded, codec.EncodedOrigin);
            using var data = (oriented ?? decoded).Encode(SKEncodedImageFormat.Jpeg, 85);

            await using var output = File.Create(filePath);
            data.SaveTo(output);
        }
        catch (Exception ex)
        {
            throw new Exception("Hiba a képfeldolgozás során.", ex);
        }

        return GetDraftImageUrl(draftId, $"{imageName}.jpg");
    }

    // The JPEG encoder drops the EXIF orientation tag, so rotate the pixels instead (e.g. phone photos).
    private static SKBitmap ApplyOrientation(SKBitmap source, SKEncodedOrigin origin)
    {
        float w = source.Width;
        float h = source.Height;

        var matrix = origin switch
        {
            SKEncodedOrigin.TopRight    => new SKMatrix(-1, 0, w, 0, 1, 0, 0, 0, 1),
            SKEncodedOrigin.BottomRight => new SKMatrix(-1, 0, w, 0, -1, h, 0, 0, 1),
            SKEncodedOrigin.BottomLeft  => new SKMatrix(1, 0, 0, 0, -1, h, 0, 0, 1),
            SKEncodedOrigin.LeftTop     => new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1),
            SKEncodedOrigin.RightTop    => new SKMatrix(0, -1, h, 1, 0, 0, 0, 0, 1),
            SKEncodedOrigin.RightBottom => new SKMatrix(0, -1, h, -1, 0, w, 0, 0, 1),
            SKEncodedOrigin.LeftBottom  => new SKMatrix(0, 1, 0, -1, 0, w, 0, 0, 1),
            _ => SKMatrix.Identity
        };

        var swapsSides = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
            or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var info = swapsSides ? source.Info.WithSize(source.Height, source.Width) : source.Info;

        var result = new SKBitmap(info);
        using var canvas = new SKCanvas(result);
        canvas.SetMatrix(matrix);
        canvas.DrawBitmap(source, 0, 0, SKSamplingOptions.Default);

        return result;
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
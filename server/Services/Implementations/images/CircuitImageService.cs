using Microsoft.AspNetCore.Hosting;
using Services.Interfaces.images;

namespace Services.Implementations.images;

public class CircuitImageService(IWebHostEnvironment env): ICircuitImagesService
{
    public string? GetImageUrl(Guid id, string imageName)
    {
        var relativePath = $"uploads/images/circuits/{id}/{imageName}.png";
       
        var physicalPath = Path.Combine(env.ContentRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        return File.Exists(physicalPath) ? $"/{relativePath}" : null;
    }
}
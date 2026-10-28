using Services.Interfaces.images;
using Services.Types;

namespace Services.Implementations.images;

public class CircuitImageService(UploadSettings uploads): ICircuitImagesService
{
    public string? GetImageUrl(Guid id, string imageName)
    {
        var physicalPath = Path.Combine(uploads.RootPath, "images", "circuits", id.ToString(), $"{imageName}.png");

        return File.Exists(physicalPath) ? $"/uploads/images/circuits/{id}/{imageName}.png" : null;
    }
}
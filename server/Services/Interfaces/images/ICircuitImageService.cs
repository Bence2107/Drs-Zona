namespace Services.Interfaces.images;

public interface ICircuitImagesService
{
    string? GetImageUrl(Guid id, string imageName);
}
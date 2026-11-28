namespace Services.Types;

// The uploads folder lives outside the content root in Docker (a volume) and can be moved out of publish/ in prod
public record UploadSettings(string RootPath);

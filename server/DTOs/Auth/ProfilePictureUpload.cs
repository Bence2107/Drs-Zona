using Microsoft.AspNetCore.Http;

namespace DTOs.Auth;

public class ProfilePictureUpload 
{
    public required IFormFile File { get; set; }
}
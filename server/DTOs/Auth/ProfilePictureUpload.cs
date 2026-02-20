using Microsoft.AspNetCore.Http;

namespace DTOs.Auth;

public class ProfilePictureUpload 
{
    public IFormFile File { get; set; }
}
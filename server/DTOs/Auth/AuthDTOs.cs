using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTOs.Auth;

public record ChangePasswordRequest(
    [Required] string CurrentPassword,
    [MinLength(8)] [Required] string NewPassword,
    [MinLength(8)] [Required] string NewPasswordAgain
);

public record LoginRequest (
    [Required] [EmailAddress] string Email,
    [Required] string Password 
);

public record RegisterRequest
(
    [Required] [MaxLength(50)] string Username,
    [Required] [MaxLength(100)] string FullName,
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(8)] string Password 
);


public record UpdateUserRequest(
    string Username,
    string FullName,
    string Email
);

public class UserProfileResponse
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public bool HasAvatar { get; set; }
    public required string Role { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public record ProfilePictureUpload(IFormFile File);

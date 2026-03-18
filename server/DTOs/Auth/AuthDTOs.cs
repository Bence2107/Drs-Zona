using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DTOs.Auth;

public record LoginRequest (
    [Required(ErrorMessage = "Email cím kitöltése kötelező")] 
    [EmailAddress(ErrorMessage = "Hibás Email formátum")] 
    string Email,
    [Required(ErrorMessage = "Jelszó megadása kötelező")]
    [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakternek kell lennie")]
    string Password 
);

public record RegisterRequest
(
    [Required(ErrorMessage = "Felhasználónév megadása kötelező")] 
    [MaxLength(50, ErrorMessage = "A felhasználónév hossza maximum 50 karakter lehet")] 
    string Username,
    [Required(ErrorMessage = "Teljes név megadása kötelező")] 
    [MaxLength(100,  ErrorMessage = "A teljes név hossza maximum 100 karakter lehet")] 
    string FullName,
    [Required(ErrorMessage = "Email cím kitöltése kötelező")] 
    [EmailAddress(ErrorMessage = "Hibás Email formátum")] 
    string Email,
    [Required(ErrorMessage = "Jelszó megadása kötelező")]
    [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakternek kell lennie")]
    string Password 
);


public record UpdateUserRequest(
    [Required(ErrorMessage = "Felhasználónév megadása kötelező")] 
    [MaxLength(50, ErrorMessage = "A felhasználónév hossza maximum 50 karakter lehet")] 
    string Username,
    [Required(ErrorMessage = "Teljes név megadása kötelező")] 
    [MaxLength(100,  ErrorMessage = "A teljes név hossza maximum 100 karakter lehet")] 
    string FullName,
    [Required(ErrorMessage = "Email cím kitöltése kötelező")] 
    [EmailAddress(ErrorMessage = "Hibás Email formátum")] 
    string Email
);

public record ChangePasswordRequest(
    [Required(ErrorMessage = "A jelenlegi jelszó megadása kötelező")] 
    string CurrentPassword,
    [Required(ErrorMessage = "Jelszó megadása kötelező")]
    [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakternek kell lennie")]
    string NewPassword,
    [Required(ErrorMessage = "Jelszó újbéli megadása kötelező")]
    [MinLength(8, ErrorMessage = "A jelszónak legalább 8 karakternek kell lennie")]
    string NewPasswordAgain
): IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NewPassword != NewPasswordAgain)
        {
            yield return new ValidationResult("A két jelszó nem egyezik",
                [nameof(NewPassword), nameof(NewPasswordAgain)]);
        }
    }
}

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

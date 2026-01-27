using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public record UserRegisterDto : IValidatableObject
{
    [Required] [StringLength(50, MinimumLength = 3)]
    public required string Username { get; init; }
    
    [Required] [EmailAddress]
    public required string Email { get; init; } 
    
    [Required] [StringLength(100, MinimumLength = 8)]
    public required string Password { get; init; } 
    
    [Required] [Compare(nameof(Password))]
    public required string ConfirmPassword { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Password.Any(char.IsUpper))
            yield return new ValidationResult("Password must contain uppercase letter", [nameof(Password)]);
        if (!Password.Any(char.IsLower))
            yield return new ValidationResult("Password must contain lowercase letter", [nameof(Password)]);
        if (!Password.Any(char.IsDigit))
            yield return new ValidationResult("Password must contain digit", [nameof(Password)]);
    }
}

public record UserLoginDto(
    [Required] string UsernameOrEmail,
    [Required] string Password
);

public record UserResponseDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    string? ProfileImageUrl,
    DateTime Created,
    DateTime LastActive
);

public record UserProfileDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    string? ProfileImageUrl,
    DateTime Created,
    DateTime LastActive,
    int TotalArticles,
    int TotalComments
);

public class UserUpdateDto : IValidatableObject
{
    [StringLength(50, MinimumLength = 3)]
    public string? Username { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    [StringLength(100, MinimumLength = 8)]
    public string? Password { get; set; }
    
    public string? ProfileImageUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Username == null && Email == null && Password == null && ProfileImageUrl == null)
            yield return new ValidationResult("At least one field must be provided");
        
        if (Password != null && !Password.Any(char.IsUpper))
            yield return new ValidationResult("Password must contain uppercase", [nameof(Password)]);
    }
}

public record UserListDto(
    Guid Id,
    string Username,
    string Email,
    string Role,
    DateTime Created,
    DateTime LastActive
);
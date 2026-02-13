using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class ChangePasswordRequest
{
    [Required]
    public required string CurrentPassword { get; set; }
    
    [Required]
    [MinLength(8)]
    public required string NewPassword { get; set; }
    
    [Required]
    [MinLength(8)]
    public required string NewPasswordAgain { get; set; }
}
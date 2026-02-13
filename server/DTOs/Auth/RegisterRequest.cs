using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string FullName { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    public required string Password { get; set; }
}
namespace DTOs.Auth;

public class UserProfileResponse
{
    
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public bool HasAvatar { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? LastActive { get; set; }
}
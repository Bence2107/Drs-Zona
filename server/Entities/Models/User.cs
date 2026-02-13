using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("username")]
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [Required]
    [Column("full_name")]
    public required string FullName { get; set; }
    
    [Required]
    [Column("email")]
    [MaxLength(100)]
    public required string Email { get; set; }
    
    [Required]
    [Column("password_hash")]
    public required string PasswordHash { get; set; }
    
    [Required]
    [Column("role")]
    [MaxLength(20)]
    public required string Role { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime Created { get; set; }
    
    [Required]
    [Column("has_avatar")]
    public bool HasAvatar { get; set; }
    
    [Column("last_active")]
    public DateTime? LastActive { get; set; }
    
    [Column("last_login")]
    public DateTime? LastLogin { get; set; } 
    
    [Column("is_logged_in")]
    public bool IsLoggedIn { get; set; }
    
    [Column("current_session_id")]
    [MaxLength(100)]
    public string? CurrentSessionId { get; set; }
}
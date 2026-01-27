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
    public required string Username { get; set; }
    
    [Required]
    [Column("email")]
    public required string Email { get; set; }
    
    [Required]
    [Column("password")]
    public required string Password { get; set; }
    
    [Required]
    [Column("role")]
    public required string Role { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime Created { get; set; }
    
    [Required]
    [Column("last_login")]
    public DateTime LastActive { get; set; }
}
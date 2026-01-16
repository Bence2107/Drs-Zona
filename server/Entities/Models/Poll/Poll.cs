using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Poll;

public class Poll
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("author_id")]
    public int AuthorId { get; set; }
    [JsonIgnore]
    public virtual User? Author { get; set; }
    
    [Required]
    [Column("description")]
    public required string Description { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }
    
    [Required]
    [Column("is_active")]
    public bool IsActive { get; set; }
}
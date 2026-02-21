using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Polls;

[Table("poll_options")]
public class PollOption
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("poll_id")]
    public Guid PollId { get; set; }
    [JsonIgnore]
    public virtual Poll? Poll { get; set; }
    
    [Required]
    [Column("text")]
    public required string Text { get; set; }
}
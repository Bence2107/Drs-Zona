using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Poll;

public class PollOption
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("text")]
    public required string Text { get; set; }
    
    [Column("poll_id")]
    public int PollId { get; set; }
    [JsonIgnore]
    public virtual Poll? Poll { get; set; }
}
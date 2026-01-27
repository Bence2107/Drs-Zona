using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Polls;

[Table("votes")]
public class Vote
{
    [Column("user_id")]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
    
    [Column("poll_option_id")]
    public Guid PollOptionId { get; set; }
    [JsonIgnore]
    public virtual PollOption? PollOption { get; set; }
}
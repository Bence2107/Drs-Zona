using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Poll;

public class Vote
{
    [Column("user_id")]
    public int UserId { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
    
    [Column("poll_option_id")]
    public int PollOptionId { get; set; }
    [JsonIgnore]
    public virtual PollOption? PollOption { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.News;

[Table("comment_votes")]
public class CommentVote
{
    [Column("user_id")]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
    
    [Column("comment_id")]
    public Guid CommentId { get; set; }
    [JsonIgnore]
    public virtual Comment? Comment { get; set; }
    
    [Column("is_upvote")]
    public bool IsUpvote { get; set; } 
}
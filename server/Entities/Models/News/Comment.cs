using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.News;

[Table("comments")]
public class Comment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }

    [Column("article_id")]
    public Guid ArticleId { get; set; }
    [JsonIgnore]
    public virtual Article? Article { get; set; }
    
    [Column("reply_id")]
    public Guid? ReplyToCommentId { get; set; }
    [JsonIgnore]
    public virtual Comment? ReplyToComment { get; set; }
    
    [Required]
    [Column("content")]
    public required string Content { get; set; }
    
    [Required]
    [Column("upvotes")]
    public int UpVotes { get; set; }
    
    [Required]
    [Column("downvotes")]
    public int DownVotes { get; set; }
    
    [Required]
    [Column("created_at")]
    public DateTime DateCreated { get; set; }
    
    [Required]
    [Column("updated_at")]
    public DateTime DateUpdated { get; set; }
}
namespace Entities.Models.News;

public class Comment
{
    public int Id { get; set; }
    
    public required string Content { get; set; }
    
    public int UpVotes { get; set; }
    
    public int DownVotes { get; set; }
    
    public DateTime DateCreated { get; set; }
    
    public DateTime DateUpdated { get; set; }
    
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    
    public int ArticleId { get; set; }
    public virtual Article? Article { get; set; }
    
    public int ReplyToCommentId { get; set; }
    public virtual Comment? ReplyToComment { get; set; }
}
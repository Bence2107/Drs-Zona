namespace Entities.Models.Poll;

public class Poll
{
    public int Id { get; set; }
    
    public int CreatorId { get; set; }
    public virtual User? Creator { get; set; }
    
    public required string Description { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsActive { get; set; }
}
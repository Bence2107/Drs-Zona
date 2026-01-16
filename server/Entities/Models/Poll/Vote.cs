namespace Entities.Models.Poll;

public class Vote
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    
    public int PollOptionId { get; set; }
    public virtual PollOption? PollOption { get; set; }
}
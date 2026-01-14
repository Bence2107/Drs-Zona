namespace Entities.Models.Poll;

public class Vote
{
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    
    public int PollId { get; set; }
    public virtual Poll? Poll { get; set; }
}
namespace Entities.Models.Poll;

public class PollOption
{
    public int Id { get; set; }
    
    public required string Text { get; set; }
    
    public int PollId { get; set; }
    public virtual Poll? Poll { get; set; }
}
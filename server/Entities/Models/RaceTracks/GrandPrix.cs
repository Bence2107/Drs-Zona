namespace Entities.Models.RaceTracks;

public class GrandPrix
{
    public int Id { get; set; }
    
    public int CircuitId { get; set; }
    public virtual Circuit? Circuit { get; set; }
    
    public required string Name { get; set; }
    
    public int RoundNumber { get; set; }
    
    public int SeasonYear { get; set; }
    
    public DateTime StartTime { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public int RaceDistance { get; set; }
    
    public int LapsCompleted { get; set; }
}
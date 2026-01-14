namespace Entities.Models.RaceTracks;

public class Circuit
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public int Length { get; set; }
    
    public required string Type { get; set; }
    
    public required string Location { get; set; }
    
    public required string FastestLap { get; set; }
}
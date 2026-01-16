namespace Entities.Models.Standings;

public class ConstructorsChampionship
{
    public int Id {get; set;}
    
    public int SeriesId { get; set; }
    public virtual Series? Series { get; set; }
    
    public required string Name { get; set; }
    
    public required string Season {get; set;}
    
    public required string Status { get; set; }
}
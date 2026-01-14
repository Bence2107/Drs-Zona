namespace Entities.Models.Standings;

public class DriversChampionship
{
    public int Id { get; set; }
   
    public required string Name { get; set; }
    
    public required string Season {get; set;}
    
    public required string Status { get; set; }
    
    public int SeriesId { get; set; }
    public virtual Series? Series { get; set; }
}
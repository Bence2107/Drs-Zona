using Entities.Models.RaceTracks;

namespace Entities.Models.Standings;

public class Result
{
    public int Id {get; set;}
    
    public int GrandPrixId {get; set;}
    public virtual GrandPrix? GrandPrix {get; set;}
    
    public int DriverId {get; set;}
    public virtual Driver? Driver {get; set;}
    
    public int TeamId {get; set;}
    public virtual Team? Team {get; set;}
    
    public int DriversChampId {get; set;}
    public virtual DriversChampionship? DriversChampionship {get; set;}
    
    public int ConsChampId {get; set;}
    public virtual ConstructorsChampionship? ConsChampionship { get; set; }

    public required string Type {get; set;}
    public long RaceTime {get; set;}
    public int DriverPoints {get; set;}
    public int TeamPoints {get; set;}
}
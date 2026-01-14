namespace Entities.Models.Standings;

public class Series
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required string GoverningBody { get; set; }
    
    public int FirstYear {get; set;}
    
    public int LastYear {get; set;}
    
    public int DriversChampId {get; set;}
    public virtual DriversChampionship? DriversChampionship {get; set;}
    
    public int ConstructorsChampId {get; set;}
    public virtual ConstructorsChampionship? ConstructorsChampionship {get; set;}
}
namespace Entities.Models.Standings;

public class Brand
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required string Description { get; set; }
    
    public required string Principal { get; set; }
    
    public required string HeadQuarters { get; set; }
}
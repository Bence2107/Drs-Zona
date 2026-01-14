namespace Entities.Models.Standings;

public class Team
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public int FoundedYear { get; set; }
    
    public required string HeadQuarters { get; set; }
    
    public required string TeamPrincipal { get; set; }
    
    public int BrandId { get; set; }
    public virtual Brand? Brand { get; set; }
    
}
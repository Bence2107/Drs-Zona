namespace Entities.Models.Standings;

public class Contract
{
    public int Id { get; set; }
    
    public int DriverId { get; set; }
    public virtual Driver? Driver { get; set; }
    
    public int TeamId { get; set; }
    public virtual Team? Team { get; set; }
}
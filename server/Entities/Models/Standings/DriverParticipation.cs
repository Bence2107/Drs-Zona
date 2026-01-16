namespace Entities.Models.Standings;

public class DriverParticipation
{
    public int DriverId { get; set; }
    public virtual Driver? Driver { get; set; }
    
    public int DriverChampId { get; set; }
    public virtual DriversChampionship? DriversChampionship { get; set; }
}
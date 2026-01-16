using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("driver_participations")]
public class DriverParticipation
{
    [Column("driver_id")]
    public int DriverId { get; set; }
    [JsonIgnore]
    public virtual Driver? Driver { get; set; }
    
    [Column("drivers_championship_id")]
    public int DriverChampId { get; set; }
    [JsonIgnore]
    public virtual DriversChampionship? DriversChampionship { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

public class ConstructorCompetition
{
    [Column("constructors_id")]
    public int ConstructorId { get; set; }
    [JsonIgnore]
    public virtual Constructor? Constructor { get; set; }
    
    [Column("constructors_championship_id")]
    public int ConstChampId { get; set; }
    [JsonIgnore]
    public virtual ConstructorsChampionship? ConstructorsChampionship { get; set; }
}

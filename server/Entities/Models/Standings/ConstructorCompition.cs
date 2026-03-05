using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("constructor_competitions")]
public class ConstructorCompetition
{
    [Column("constructors_id")]
    public Guid ConstructorId { get; set; }
    [JsonIgnore]
    public virtual Constructor? Constructor { get; set; }
    
    [Column("constructors_championship_id")]
    public Guid ConstChampId { get; set; }
    [JsonIgnore]
    public virtual ConstructorsChampionship? ConstructorsChampionship { get; set; }
    
    [Column("constructor_name_snapshot")]
    public required string ConstructorNameSnapshot { get; set; }

    [Column("constructor_nickname_snapshot")]
    public required string ConstructorNicknameSnapshot { get; set; }
}

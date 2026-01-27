using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.Standings;

namespace Entities.Models.RaceTracks;

[Table("grands_prix")]
public class GrandPrix
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("circuit_id")]
    public Guid CircuitId { get; set; }
    [JsonIgnore]
    public virtual Circuit? Circuit { get; set; }
    
    [Column("series_id")]
    public Guid SeriesId { get; set; }
    [JsonIgnore]
    public virtual Series? Series { get; set; } 
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("round_number")]
    public int RoundNumber { get; set; }
    
    [Required]
    [Column("season_year")]
    public int SeasonYear { get; set; }
    
    [Required]
    [Column("start_time")]
    public DateTime StartTime { get; set; }
    
    [Required]
    [Column("end_time")]
    public DateTime EndTime { get; set; }
    
    [Required]
    [Column("race_distance")]
    public int RaceDistance { get; set; }
    
    [Required]
    [Column("laps_completed")]
    public int LapsCompleted { get; set; }
}
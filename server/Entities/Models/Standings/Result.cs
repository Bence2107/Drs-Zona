using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.RaceTracks;

namespace Entities.Models.Standings;

[Table("results")]
public class Result
{
    [Key]
    [Column("id")]
    public Guid Id {get; set;}
    
    [Required]
    [Column("grand_prix_id")]
    public Guid GrandPrixId {get; set;}
    [JsonIgnore]
    public virtual GrandPrix? GrandPrix {get; set;}
    
    [Required]
    [Column("driver_id")]
    public Guid DriverId {get; set;}
    [JsonIgnore]
    public virtual Driver? Driver {get; set;}
    
    [Column("constructor_id")]
    public Guid ConstructorId {get; set;}
    [JsonIgnore]
    public virtual Constructor? Constructor {get; set;}
    
    [Column("drivers_championship_id")]
    public Guid DriversChampId {get; set;}
    [JsonIgnore]
    public virtual DriversChampionship? DriversChampionship {get; set;}
    
    [Column("constructors_championship_id")]
    public Guid ConsChampId {get; set;}
    [JsonIgnore]
    public virtual ConstructorsChampionship? ConsChampionship { get; set; }
    
    [Required]
    [Column("start_position")]
    public int StartPosition {get; set;}
    
    [Required]
    [Column("finish_position")]
    public int FinishPosition {get; set;}

    [Required]
    [Column("session")]
    public required string Session {get; set;}
    
    [Required]
    [Column("race_time")]
    public long RaceTime {get; set;}
    
    [Required]
    [Column("status")]
    public required string Status {get; set;}
    
    [Required]
    [Column("laps_completed")]
    public int LapsCompleted {get; set;}
    
    [Required]
    [Column("driver_points")]
    public int DriverPoints {get; set;}
    
    [Required]
    [Column("constructor_points")]
    public int ConstructorPoints {get; set;}
}
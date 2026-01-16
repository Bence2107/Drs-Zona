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
    public int Id {get; set;}
    
    [Required]
    [Column("grand_prix_id")]
    public int GrandPrixId {get; set;}
    [JsonIgnore]
    public virtual GrandPrix? GrandPrix {get; set;}
    
    [Required]
    [Column("driver_id")]
    public int DriverId {get; set;}
    [JsonIgnore]
    public virtual Driver? Driver {get; set;}
    
    [Column("constructor_id")]
    public int ConstructorId {get; set;}
    [JsonIgnore]
    public virtual Constructor? Constructor {get; set;}
    
    [Column("drivers_championship_id")]
    public int DriversChampId {get; set;}
    [JsonIgnore]
    public virtual DriversChampionship? DriversChampionship {get; set;}
    
    [Column("constructors_championship_id")]
    public int ConsChampId {get; set;}
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
    [Column("driver_points")]
    public int DriverPoints {get; set;}
    
    [Required]
    [Column("constructor_points")]
    public int ConstructorPoints {get; set;}
}
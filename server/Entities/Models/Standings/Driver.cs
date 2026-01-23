using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Standings;

[Table("drivers")]
public class Driver
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("nationality")]
    public required string Nationality { get; set; }
    
    [Required]
    [Column("birth_date")]
    public DateTime BirthDate { get; set; }
    
    [Required]
    [Column("driver_number")]
    public int DriverNumber { get; set; }
    
    [Required]
    [Column("total_races")]
    public int TotalRaces { get; set; }
    
    [Required]
    [Column("wins")]
    public int Wins { get; set; }
    
    [Required]
    [Column("podiums")]
    public int Podiums { get; set; }
    
    [Required]
    [Column("championships")]
    public int Championships { get; set; }
    
    [Required]
    [Column("pole_positions")]
    public int PolePositions { get; set; }
   
    [Required]
    [Column("seasons")]
    public int Seasons { get; set; }
   
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.RaceTracks;

public class Circuit
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("length")]
    public int Length { get; set; }
    
    [Required]
    [Column("type")]
    public required string Type { get; set; }
    
    [Required]
    [Column("location")]
    public required string Location { get; set; }
    
    [Required]
    [Column("fastest_lap")]
    public required string FastestLap { get; set; }
}
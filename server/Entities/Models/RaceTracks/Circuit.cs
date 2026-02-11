using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.RaceTracks;

[Table("circuits")]
public class Circuit
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("length")]
    public double Length { get; set; }
    
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
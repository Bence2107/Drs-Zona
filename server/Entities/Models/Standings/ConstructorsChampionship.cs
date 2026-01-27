using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("constructor_champions")]
public class ConstructorsChampionship
{
    [Key]
    [Column("id")]
    public Guid Id {get; set;}
    
    [Column("series_id")]
    public Guid SeriesId { get; set; }
    [JsonIgnore]
    public virtual Series? Series { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("season")]
    public required string Season {get; set;}
    
    [Required]
    [Column("status")]
    public required string Status { get; set; }
}
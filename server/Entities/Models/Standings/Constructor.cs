using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("constructors")]
public class Constructor
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("brand_id")]
    public Guid BrandId { get; set; }
    [JsonIgnore]
    public virtual Brand? Brand { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("nickname")]
    public required string Nickname { get; set; }
    
    [Required]
    [Column("founded_year")]
    public int FoundedYear { get; set; }
    
    [Required]
    [Column("headquarters")]
    public required string? HeadQuarters { get; set; }
    
    [Required]
    [Column("team_chief")]
    public required string TeamChief { get; set; }
    
    [Required]
    [Column("technical_chief")]
    public required string TechnicalChief { get; set; }
    
    [Required]
    [Column("seasons")]
    public int Seasons { get; set; }
    
    [Required]
    [Column("championships")]
    public int Championships { get; set; }
    
    [Required]
    [Column("wins")]
    public int Wins { get; set; }
    
    [Required]
    [Column("podiums")]
    public int Podiums { get; set; }
}
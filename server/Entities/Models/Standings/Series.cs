using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Standings;

[Table("series")]
public class Series
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("description")]
    public required string Description { get; set; }
    
    [Required]
    [Column("governing_body")]
    public required string GoverningBody { get; set; }
    
    [Required]
    [Column("first_year ")]
    public int FirstYear {get; set;}
    
    [Required]
    [Column("last_year ")]
    public int LastYear {get; set;}
}
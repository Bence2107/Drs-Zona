using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Standings;

[Table("brands")]
public class Brand
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("name")]
    public required string Name { get; set; }
    
    [Required]
    [Column("description")]
    public required string Description { get; set; }
    
    [Required]
    [Column("principal")]
    public required string Principal { get; set; }
    
    [Required]
    [Column("headquarters")]
    public required string HeadQuarters { get; set; }
}
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
}
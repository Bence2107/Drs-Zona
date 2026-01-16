using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("contracts")]
public class Contract
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("driver_id")]
    public int DriverId { get; set; }
    [JsonIgnore]
    public virtual Driver? Driver { get; set; }
    
    [Column("constructor_id")]
    public int ConstructorId { get; set; }
    [JsonIgnore]
    public virtual Constructor? Constructor { get; set; }
}
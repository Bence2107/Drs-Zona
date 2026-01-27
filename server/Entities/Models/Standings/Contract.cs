using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("contracts")]
public class Contract
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("driver_id")]
    public Guid DriverId { get; set; }
    [JsonIgnore]
    public virtual Driver? Driver { get; set; }
    
    [Column("constructor_id")]
    public Guid ConstructorId { get; set; }
    [JsonIgnore]
    public virtual Constructor? Constructor { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

[Table("qualifying_results")]
public class QualifyingResult
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("result_id")]
    public Guid ResultId { get; set; }

    [JsonIgnore]
    public virtual Result? Result { get; set; }

    [Column("q1_time")]
    public long? Q1 { get; set; }

    [Column("q2_time")]
    public long? Q2 { get; set; }

    [Column("q3_time")]
    public long? Q3 { get; set; }
}
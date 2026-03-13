using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entities.Models.Standings;

/// <summary>
/// WEC-specifikus tábla: egy autóhoz (Result) tartozó összes pilóta.
/// Egy Result sor = egy autó/pozíció. A co-driverek itt vannak felsorolva.
/// </summary>
[Table("car_entries")]
public class CarEntry
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// A futam eredménysora (az autó pozíciója, pontjai stb.)
    /// </summary>
    [Required]
    [Column("result_id")]
    public Guid ResultId { get; set; }
    [JsonIgnore]
    public virtual Result? Result { get; set; }

    /// <summary>
    /// Az adott pilóta aki ebben az autóban volt
    /// </summary>
    [Required]
    [Column("driver_id")]
    public Guid? DriverId { get; set; }
    [JsonIgnore]
    public virtual Driver? Driver { get; set; }

    /// <summary>
    /// Snapshot a pilóta nevéről a rögzítés pillanatában
    /// </summary>
    [Required]
    [Column("driver_name_snapshot")]
    public required string DriverNameSnapshot { get; set; }

    /// <summary>
    /// true = ez a pilóta vezette a Hyperpole/Qualifying kört
    /// (időmérő álláshoz ez a sor számít)
    /// </summary>
    [Column("is_qualifier")]
    public bool IsQualifier { get; set; } = false;
}
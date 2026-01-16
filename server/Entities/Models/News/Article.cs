using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Entities.Models.RaceTracks;

namespace Entities.Models.News;

[Table("articles")]
public class Article
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("author_id")]
    public int AuthorId { get; set; }
    [JsonIgnore]
    public virtual User? Author { get; set; }
    
    [Column("grand_prix_id")]
    public int? GrandPrixId { get; set; }
    [JsonIgnore]
    public virtual GrandPrix? GrandPrix { get; set; }
    
    [Required]
    [Column("title")]
    public required string Title { get; set; }
    
    [Required]
    [Column("lead")]
    public required string Lead { get; set; }
    
    [Required]
    [Column("content")]
    public required string Content { get; set; }
    
    [Required]
    [Column("published_at")]
    public DateTime DatePublished { get; set; }
    
    [Required]
    [Column("updated_at")]
    public DateTime DateUpdated { get; set; }
}
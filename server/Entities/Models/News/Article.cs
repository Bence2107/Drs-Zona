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
    public Guid Id { get; set; }
    
    [Column("author_id")]
    public Guid AuthorId { get; set; }
    [JsonIgnore]
    public virtual User? Author { get; set; }
    
    [Column("grand_prix_id")]
    public Guid? GrandPrixId { get; set; }
    [JsonIgnore]
    public virtual GrandPrix? GrandPrix { get; set; }
    
    [Required]
    [Column("title")]
    public required string Title { get; set; }
    
    [Required]
    [Column("lead")]
    public required string Lead { get; set; }
    
    [Required]
    [Column("slug")]
    public required string Slug { get; set; }
    
    [Required]
    [Column("first_section")]
    public required string FirstSection { get; set; }
    
    [Column("second_section")]
    public string? SecondSection { get; set; }    
    
    [Column("third_section")]
    public string? ThirdSection { get; set; }    
    
    [Column("fourth_section")]
    public string? FourthSection { get; set; } 
    
    [Required]
    [Column("last_section")]
    public required string LastSection { get; set; }
    
    [Required]
    [Column("published_at")]
    public DateTime DatePublished { get; set; }
    
    [Required]
    [Column("updated_at")]
    public DateTime DateUpdated { get; set; }
}
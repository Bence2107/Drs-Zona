using Entities.Models.RaceTracks;

namespace Entities.Models.News;

public class Article
{
    public int Id { get; set; }
    
    public int AuthorId { get; set; }
    public virtual User? Author { get; set; }
    
    public int? GrandPrixId { get; set; }
    public virtual GrandPrix? GrandPrix { get; set; }
    
    public required string Title { get; set; }
    
    public required string Lead { get; set; }
    
    public required string Content { get; set; }
    
    public DateTime DatePublished { get; set; }
    
    public DateTime DateModified { get; set; }
}
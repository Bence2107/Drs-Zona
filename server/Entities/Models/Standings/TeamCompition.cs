namespace Entities.Models.Standings;

public class TeamCompetition
{
    public int TeamId { get; set; }
    public virtual Team? Team { get; set; }
    
    public int ConstChampId { get; set; }
    public virtual ConstructorsChampionship? ConstructorsChampionship { get; set; }
}

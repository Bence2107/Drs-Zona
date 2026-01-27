using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface ISeriesRepository
{
    Series? GetSeriesById(Guid id);
    List<Series> GetAllSeries();
    void Create(Series series);
    void Update(Series series);
    void Delete(Guid id);
    Series? GetByName(string name);
    bool CheckIfIdExists(Guid id);
}
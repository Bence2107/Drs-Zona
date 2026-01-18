using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface ISeriesRepository
{
    Series? GetSeriesById(int id);
    List<Series> GetAllSeries();
    void Create(Series series);
    void Update(Series series);
    void Delete(int id);
    Series? GetByName(string name);
    bool CheckIfIdExists(int id);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface ISeriesRepository 
{
    Task<Series?> GetSeriesById(Guid id);
    Task<List<Series>> GetAllSeries();
    Task<Series?> GetByName(string name);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface ISeriesRepository
{
    Task<Series?> GetSeriesById(Guid id);
    Task<List<Series>> GetAllSeries();
    Task Create(Series series);
    Task Update(Series series);
    Task Delete(Guid id);
    Task<Series?> GetByName(string name);
    Task<bool> CheckIfIdExists(Guid id);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversChampionshipsRepository
{
    Task<DriversChampionship?> GetById(Guid id);
    Task<List<DriversChampionship>> GetAll();
    Task<List<DriversChampionship>> GetBySeriesId(Guid seriesId);
    Task Add(DriversChampionship championship);
    Task Modify(DriversChampionship championship);
    Task Delete(Guid id);
    Task<DriversChampionship?> GetByIdWithSeries(Guid id);
    Task<List<DriversChampionship>> GetBySeason(string season);
    Task<List<DriversChampionship>> GetByStatus(string status);
    Task<bool> CheckIfIdExists(Guid id);
}
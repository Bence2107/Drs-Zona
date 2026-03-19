using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversChampionshipsRepository
{
    Task<DriversChampionship?> GetById(Guid id);
    Task<List<DriversChampionship>> GetBySeriesId(Guid seriesId);
    Task Create(DriversChampionship championship);
    Task Modify(DriversChampionship championship);
    Task<DriversChampionship?> GetByIdWithSeries(Guid id);
}
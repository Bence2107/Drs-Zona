using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversChampionshipsRepository
{
    DriversChampionship? GetById(Guid id);
    List<DriversChampionship> GetAll();
    List<DriversChampionship> GetBySeriesId(Guid seriesId);
    void Add(DriversChampionship championship);
    void Modify(DriversChampionship championship);
    void Delete(Guid id);
    DriversChampionship? GetByIdWithSeries(Guid id);
    List<DriversChampionship> GetBySeason(string season);
    List<DriversChampionship> GetByStatus(string status);
    bool CheckIfIdExists(Guid id);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversChampionshipsRepository
{
    DriversChampionship? GetById(int id);
    List<DriversChampionship> GetAll();
    List<DriversChampionship> GetBySeriesId(int seriesId);
    void Add(DriversChampionship championship);
    void Modify(DriversChampionship championship);
    void Delete(int id);
    DriversChampionship? GetByIdWithSeries(int id);
    List<DriversChampionship> GetBySeason(string season);
    List<DriversChampionship> GetByStatus(string status);
    bool CheckIfIdExists(int id);
}
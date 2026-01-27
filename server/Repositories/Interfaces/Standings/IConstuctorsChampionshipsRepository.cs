using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsChampionshipsRepository
{
    ConstructorsChampionship? GetAllConstructorsChampionshipById(Guid id);
    List<ConstructorsChampionship> GetAllConstructorsChampionships();
    List<ConstructorsChampionship> GetBySeriesId(Guid seriesId);
    void Create(ConstructorsChampionship constructorsChampionship);
    void Update(ConstructorsChampionship constructorsChampionship);
    void Delete(Guid id);
    ConstructorsChampionship? GetByIdWithSeries(Guid id);
    List<ConstructorsChampionship> GetBySeason(string season);
    List<ConstructorsChampionship> GetByStatus(string status);
    bool CheckIfIdExists(Guid id);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsChampionshipsRepository
{
    ConstructorsChampionship? GetAllConstructorsChampionshipById(int id);
    List<ConstructorsChampionship> GetAllConstructorsChampionships();
    void Create(ConstructorsChampionship constructorsChampionship);
    void Update(ConstructorsChampionship constructorsChampionship);
    void Delete(int id);
    ConstructorsChampionship? GetByIdWithSeries(int id);
    List<ConstructorsChampionship> GetBySeason(string season);
    List<ConstructorsChampionship> GetByStatus(string status);
    bool CheckIfIdExists(int id);
}
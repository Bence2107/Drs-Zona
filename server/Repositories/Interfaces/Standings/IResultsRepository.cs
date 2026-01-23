using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IResultsRepository
{
    Result? GetResultById(int id);
    List<Result> GetAllResults();
    void Create(Result result);
    void Update(Result result);
    void Delete(int id);
    Result? GetResultWithAll(int id);
    List<Result> GetByGrandPrixId(int grandPrixId);
    List<Result> GetByDriverId(int driverId);
    List<Result> GetByConstructorId(int constructorId);
    List<Result> GetBySession(string session);
    List<Result> GetByDriversChampionshipId(int championshipId);
    List<Result> GetByConstructorsChampionshipId(int championshipId);
    bool CheckIfIdExists(int id);
}
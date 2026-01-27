using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IResultsRepository
{
    Result? GetResultById(Guid id);
    List<Result> GetAllResults();
    void Create(Result result);
    void Update(Result result);
    void Delete(Guid id);
    Result? GetResultWithAll(Guid id);
    List<Result> GetByGrandPrixId(Guid grandPrixId);
    List<Result> GetByDriverId(Guid driverId);
    List<Result> GetByConstructorId(Guid constructorId);
    List<Result> GetBySession(string session);
    List<Result> GetByDriversChampionshipId(Guid championshipId);
    List<Result> GetByConstructorsChampionshipId(Guid championshipId);
    bool CheckIfIdExists(Guid id);
}
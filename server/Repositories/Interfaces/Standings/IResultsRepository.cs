using Entities.Models.RaceTracks;
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IResultsRepository 
{
    Task<Result?> GetResultById(Guid id);
    Task<List<Result>> GetByGrandPrixId(Guid grandPrixId);
    Task<List<Result>> GetBySession(Guid grandPrixId, string session);
    Task<List<Result>> GetByDriversChampionshipId(Guid championshipId);
    Task<List<Result>> GetByConstructorsChampionshipId(Guid championshipId);
    Task<List<string>> GetAvailableSessionsByGrandPrixId(Guid grandPrixId);
    Task Create(Result result);
    Task Update(Result result);
    Task Delete(Guid id);
    Task<bool> HasGrandPrixResults(GrandPrix gp);
    Task<Result?> GetByGpDriverSession(Guid grandPrixId, Guid driverId, string session);
}
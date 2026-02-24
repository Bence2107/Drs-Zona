using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorCompetitionRepository
{
    Task<ConstructorCompetition?> GetConstructorCompetitionById(Guid constructorId, Guid championshipId);
    Task<List<ConstructorCompetition>> GetAllConstructorCompetitions();
    Task Create(ConstructorCompetition constructorCompetition);
    Task Update(ConstructorCompetition constructorCompetition);
    Task Delete(Guid constructorId, Guid championshipId);
    Task<List<ConstructorCompetition>> GetByConstructorId(Guid constructorId);
    Task<List<ConstructorCompetition>> GetByChampionshipId(Guid champId);
    Task<List<Constructor?>> GetConstructorsByChampionshipId(Guid championshipId);
    Task<bool> CheckIfExists(Guid constructorId, Guid champId);
}
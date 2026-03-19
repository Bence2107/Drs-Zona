using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorCompetitionRepository 
{
    Task<ConstructorCompetition?> GetByConstructorAndChampionship(Guid constructorId, Guid championshipId);
    Task<List<ConstructorCompetition>> GetByChampionshipId(Guid champId);
    Task<List<Constructor?>> GetConstructorsByChampionshipId(Guid championshipId);
    Task Create(ConstructorCompetition constructorCompetition);
    Task Delete(Guid constructorId, Guid championshipId);
    Task<bool> CheckIfExists(Guid constructorId, Guid champId);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorCompetitionRepository
{
    ConstructorCompetition? GetConstructorCompetitionById(Guid constructorId, Guid championshipId);
    List<ConstructorCompetition> GetAllConstructorCompetitions();
    void Create(ConstructorCompetition constructorCompetition);
    void Update(ConstructorCompetition constructorCompetition);
    void Delete(Guid constructorId, Guid championshipId);
    List<ConstructorCompetition> GetByConstructorId(Guid constructorId);
    List<ConstructorCompetition> GetByChampionshipId(Guid champId);
    List<Constructor?> GetConstructorsByChampionshipId(Guid championshipId);
    bool CheckIfExists(Guid constructorId, Guid champId);
}
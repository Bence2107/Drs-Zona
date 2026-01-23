using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorCompetitionRepository
{
    ConstructorCompetition? GetConstructorCompetitionById(int constructorId, int championshipId);
    List<ConstructorCompetition> GetAllConstructorCompetitions();
    void Create(ConstructorCompetition constructorCompetition);
    void Update(ConstructorCompetition constructorCompetition);
    void Delete(int constructorId, int championshipId);
    List<ConstructorCompetition> GetByConstructorId(int constructorId);
    List<ConstructorCompetition> GetByChampionshipId(int champId);
    List<Constructor?> GetConstructorsByChampionshipId(int championshipId);
    bool CheckIfExists(int constructorId, int champId);
}
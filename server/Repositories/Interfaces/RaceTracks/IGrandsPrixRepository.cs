using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface IGrandsPrixRepository
{
    GrandPrix? GetGrandPrixById(int id);
    List<GrandPrix> GetAllGrandPrix();
    void Create(GrandPrix grandPrix);
    void Update(GrandPrix grandPrix);
    void Delete(int id);
    GrandPrix? GetByIdWithCircuit(int id);
    List<GrandPrix> GetBySeason(int year);
    List<GrandPrix> GetByCircuitId(int circuitId);
    GrandPrix? GetByRoundAndSeason(int round, int season);
    bool CheckIfIdExists(int id);   
}
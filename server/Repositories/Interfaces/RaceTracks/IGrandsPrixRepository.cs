using Entities.Models.RaceTracks;
using Entities.Models.Standings;

namespace Repositories.Interfaces.RaceTracks;

public interface IGrandsPrixRepository
{
    GrandPrix? GetGrandPrixById(int id);
    List<GrandPrix> GetAllGrandPrix();
    void Create(GrandPrix grandPrix);
    void Update(GrandPrix grandPrix);
    void Delete(int id);
    GrandPrix? GetByIdWithCircuit(int id);
    GrandPrix? GetWithAll(int id);
    List<GrandPrix> GetBySeason(int year);
    List<GrandPrix> GetByCircuitId(int circuitId);
    List<GrandPrix> GetBySeriesAndYear(int seriesId, int year);
    GrandPrix? GetByRoundAndSeason(int round, int season);
    bool CheckIfIdExists(int id);   
}
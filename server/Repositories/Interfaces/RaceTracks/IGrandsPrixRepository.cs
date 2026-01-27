using Entities.Models.RaceTracks;
using Entities.Models.Standings;

namespace Repositories.Interfaces.RaceTracks;

public interface IGrandsPrixRepository
{
    GrandPrix? GetGrandPrixById(Guid id);
    List<GrandPrix> GetAllGrandPrix();
    void Create(GrandPrix grandPrix);
    void Update(GrandPrix grandPrix);
    void Delete(Guid id);
    GrandPrix? GetByIdWithCircuit(Guid id);
    GrandPrix? GetWithAll(Guid id);
    List<GrandPrix> GetBySeason(int year);
    List<GrandPrix> GetByCircuitId(Guid circuitId);
    List<GrandPrix> GetBySeriesAndYear(Guid seriesId, int year);
    GrandPrix? GetByRoundAndSeason(int round, int season);
    bool CheckIfIdExists(Guid id);   
}
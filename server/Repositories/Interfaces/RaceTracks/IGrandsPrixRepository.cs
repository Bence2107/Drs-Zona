using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface IGrandsPrixRepository
{
    Task<GrandPrix?> GetGrandPrixById(Guid id);
    Task<List<GrandPrix>> GetAllGrandPrix();
    Task Create(GrandPrix grandPrix);
    Task Update(GrandPrix grandPrix);
    Task Delete(Guid id);
    Task<GrandPrix?> GetByIdWithCircuit(Guid id);
    Task<GrandPrix?> GetWithAll(Guid id);
    Task<List<GrandPrix>> GetBySeason(int year);
    Task<List<GrandPrix>> GetByCircuitId(Guid circuitId);
    Task<List<GrandPrix>> GetBySeriesAndYear(Guid seriesId, int year);
    Task<GrandPrix?> GetByRoundAndSeason(int round, int season);
    Task<bool> CheckIfIdExists(Guid id);   
}
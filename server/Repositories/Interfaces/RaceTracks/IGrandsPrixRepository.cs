using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface IGrandsPrixRepository 
{
    Task<GrandPrix?> GetGrandPrixById(Guid id);
    Task<GrandPrix?> GetWithAll(Guid id);
    Task<List<GrandPrix>> GetBySeriesAndYear(Guid seriesId, int year);
    Task Create(GrandPrix grandPrix);
    Task Update(GrandPrix grandPrix);
    Task Delete(Guid id);
    Task<bool> CheckIfIdExists(Guid id);   
}
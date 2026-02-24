using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsChampionshipsRepository
{
    Task<ConstructorsChampionship?> GetAllConstructorsChampionshipById(Guid id);
    Task<List<ConstructorsChampionship>> GetAllConstructorsChampionships();
    Task<List<ConstructorsChampionship>> GetBySeriesId(Guid seriesId);
    Task Create(ConstructorsChampionship constructorsChampionship);
    Task Update(ConstructorsChampionship constructorsChampionship);
    Task Delete(Guid id);
    Task<ConstructorsChampionship?> GetByIdWithSeries(Guid id);
    Task<List<ConstructorsChampionship>> GetBySeason(string season);
    Task<List<ConstructorsChampionship>> GetByStatus(string status);
    Task<bool> CheckIfIdExists(Guid id);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IConstructorsChampionshipsRepository 
{
    Task<List<ConstructorsChampionship>> GetBySeriesId(Guid seriesId);
    Task Create(ConstructorsChampionship constructorsChampionship);
    Task Update(ConstructorsChampionship constructorsChampionship);
    Task<ConstructorsChampionship?> GetByIdWithSeries(Guid id);
}
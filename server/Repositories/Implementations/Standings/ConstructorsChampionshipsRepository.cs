using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorsChampionshipsRepository(EfContext context) : IConstructorsChampionshipsRepository 
{
    private readonly DbSet<ConstructorsChampionship> _constructorsChampionships = context.ConstructorsChampionships;
    
    public async Task<List<ConstructorsChampionship>> GetBySeriesId(Guid seriesId) =>
        await _constructorsChampionships
            .Include(c => c.Series)
            .Where(c => c.SeriesId == seriesId).ToListAsync();

    public async Task Create(ConstructorsChampionship constructorsChampionship)
    {
        _constructorsChampionships.Add(constructorsChampionship);
        await context.SaveChangesAsync();
    }

    public async Task Update(ConstructorsChampionship constructorsChampionship)
    {
        _constructorsChampionships.Update(constructorsChampionship);
        await context.SaveChangesAsync();
    }
    
    public async Task<ConstructorsChampionship?> GetByIdWithSeries(Guid id) => await _constructorsChampionships
        .Include(cc => cc.Series)
        .FirstOrDefaultAsync(cc => cc.Id == id);

}
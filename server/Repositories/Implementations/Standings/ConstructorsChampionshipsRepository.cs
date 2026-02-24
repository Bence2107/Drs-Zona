using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorsChampionshipsRepository(EfContext context) : IConstructorsChampionshipsRepository
{
    private readonly DbSet<ConstructorsChampionship> _constructorsChampionships = context.ConstructorsChampionships;

    public async Task<ConstructorsChampionship?> GetAllConstructorsChampionshipById(Guid id) => 
        await _constructorsChampionships.FirstOrDefaultAsync(cc => cc.Id == id);

    public async Task<List<ConstructorsChampionship>> GetAllConstructorsChampionships() => 
        await _constructorsChampionships.ToListAsync();
    
    public async Task<List<ConstructorsChampionship>> GetBySeriesId(Guid seriesId) =>
        await _constructorsChampionships.Where(c => c.SeriesId == seriesId).ToListAsync();

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

    public async Task Delete(Guid id)
    {
        var constructorsChampionship = await GetAllConstructorsChampionshipById(id);
        if (constructorsChampionship == null) return;

        _constructorsChampionships.Remove(constructorsChampionship);
        await context.SaveChangesAsync();
    }

    public async Task<ConstructorsChampionship?> GetByIdWithSeries(Guid id) => await _constructorsChampionships
        .Include(cc => cc.Series)
        .FirstOrDefaultAsync(cc => cc.Id == id);

    public async Task<List<ConstructorsChampionship>> GetBySeason(string season) => await _constructorsChampionships
        .Where(cc => cc.Season == season)
        .ToListAsync();

    public async Task<List<ConstructorsChampionship>> GetByStatus(string status) => await _constructorsChampionships
        .Where(cc => cc.Status == status)
        .ToListAsync();

    public async Task<bool> CheckIfIdExists(Guid id) => await _constructorsChampionships.AnyAsync(cc => cc.Id == id);
}
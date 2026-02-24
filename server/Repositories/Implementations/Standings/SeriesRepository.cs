using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class SeriesRepository(EfContext context) : ISeriesRepository
{
    private readonly DbSet<Series> _series = context.Series;
    
    public async Task<Series?> GetSeriesById(Guid id) => await _series.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<List<Series>> GetAllSeries() => await _series.ToListAsync();

    public async Task Create(Series series)
    {
        await _series.AddAsync(series);
        await context.SaveChangesAsync();
    }

    public async Task Update(Series series)
    {
        _series.Update(series);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var series = await GetSeriesById(id);
        if (series == null) return;
        
        _series.Remove(series);
        await context.SaveChangesAsync();
    }

    public async Task<Series?> GetByName(string name) => await _series.FirstOrDefaultAsync(d => d.Name == name);

    public async Task<bool> CheckIfIdExists(Guid id) => await _series.AnyAsync(d => d.Id == id);
}
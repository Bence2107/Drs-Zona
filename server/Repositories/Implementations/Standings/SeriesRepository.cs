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

    public async Task<Series?> GetByName(string name) => await _series.FirstOrDefaultAsync(d => d.ShortName == name);

}
using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriversChampionshipsRepository(EfContext context) : IDriversChampionshipsRepository
{
    private readonly DbSet<DriversChampionship> _driversChampionships = context.DriversChampionships;
    
    public async Task<DriversChampionship?> GetById(Guid id) => await _driversChampionships.FirstOrDefaultAsync(d => d.Id == id);
    

    public async Task<List<DriversChampionship>> GetBySeriesId(Guid seriesId) =>
        await _driversChampionships
            .Include(c => c.Series)
            .Where(c => c.SeriesId == seriesId).ToListAsync();

    public async Task Create(DriversChampionship championship)
    {
        await _driversChampionships.AddAsync(championship);
        await context.SaveChangesAsync();
    }

    public async Task Modify(DriversChampionship championship)
    {
        _driversChampionships.Update(championship);
        await context.SaveChangesAsync();
    }


    public async Task<DriversChampionship?> GetByIdWithSeries(Guid id) => await _driversChampionships
        .Include(d => d.Series)
        .FirstOrDefaultAsync(d => d.Id == id);
    
}
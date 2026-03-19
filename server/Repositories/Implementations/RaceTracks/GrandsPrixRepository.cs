using Context;
using Entities.Models.RaceTracks;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.RaceTracks;

namespace Repositories.Implementations.RaceTracks;

public class GrandsPrixRepository(EfContext context) : IGrandsPrixRepository 
{
    private readonly DbSet<GrandPrix> _grandsPrix = context.GrandsPrix;
    
    public async Task<GrandPrix?> GetGrandPrixById(Guid id) => 
        await _grandsPrix.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<GrandPrix?> GetWithAll(Guid id) => 
        await _grandsPrix
            .Include(c => c.Circuit)
            .Include(c => c.Series)
            .FirstOrDefaultAsync(c => c.Id == id);
    
    public async Task<List<GrandPrix>> GetBySeriesAndYear(Guid seriesId, int year) => 
        await _grandsPrix
            .Include(gp => gp.Circuit)
            .Where(gp => gp.SeriesId == seriesId && gp.SeasonYear == year)
            .ToListAsync();
    
    public async Task Create(GrandPrix grandPrix)
    {
        await _grandsPrix.AddAsync(grandPrix);
        await context.SaveChangesAsync();
    }

    public async Task Update(GrandPrix grandPrix)
    {
        _grandsPrix.Update(grandPrix);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var grandPrix = await GetGrandPrixById(id);
        if (grandPrix == null) return;
        
        _grandsPrix.Remove(grandPrix);
        await context.SaveChangesAsync();
    }
    
    public async Task<bool> CheckIfIdExists(Guid id) => 
        await _grandsPrix.AnyAsync(c => c.Id == id);
}
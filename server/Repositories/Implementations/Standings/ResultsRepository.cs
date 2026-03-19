using Context;
using Entities.Models.RaceTracks;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ResultsRepository(EfContext context) : IResultsRepository 
{
    private readonly DbSet<Result> _results = context.Results;

    public async Task<Result?> GetResultById(Guid id) => await _results.FirstOrDefaultAsync(r => r.Id == id);
    
    public async Task<List<Result>> GetByGrandPrixId(Guid grandPrixId) => await _results
        .Where(r => r.GrandPrixId == grandPrixId)
        .ToListAsync();

    public async Task<List<Result>> GetBySession(Guid grandPrixId, string session) => await _results
        .Include(r => r.GrandPrix)
        .Where(r => r.GrandPrixId == grandPrixId && r.Session == session)
        .ToListAsync();

    public async Task<List<Result>> GetByDriversChampionshipId(Guid championshipId) => await _results
        .Include(r => r.Driver)
        .Include(r => r.GrandPrix)
        .Include(r => r.Constructor)
        .Where(r=> r.DriversChampId == championshipId)
        .ToListAsync();
    
    public async Task<List<Result>> GetByConstructorsChampionshipId(Guid championshipId) => await _results
        .Include(r => r.Constructor)
        .Include(r => r.GrandPrix)
        .Where(r=> r.ConsChampId == championshipId)
        .ToListAsync();

    public async Task<List<string>> GetAvailableSessionsByGrandPrixId(Guid grandPrixId)
    {
        return await _results
            .AsNoTracking()
            .Where(r => r.GrandPrixId == grandPrixId)
            .Select(r => r.Session)
            .Distinct() 
            .OrderBy(s => s) 
            .ToListAsync();
    }
    
    public async Task Create(Result result)
    {
        await _results.AddAsync(result);
        await context.SaveChangesAsync();
    }

    public async Task Update(Result result)
    {
        _results.Update(result);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var result = await GetResultById(id);
        if(result == null) return;
        
        _results.Remove(result);
        await context.SaveChangesAsync();
    }
    
    public async Task<bool> HasGrandPrixResults(GrandPrix gp)
    {
        return await _results.AnyAsync(r => r.GrandPrixId == gp.Id);
    }

    public async Task<Result?> GetByGpDriverSession(Guid grandPrixId, Guid driverId, string session)
    {
        return await _results.FirstOrDefaultAsync(
            r => 
                r.GrandPrixId == grandPrixId && 
                r.DriverId == driverId && 
                r.Session == session
        );
    }
}
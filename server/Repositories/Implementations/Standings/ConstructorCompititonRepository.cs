using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorCompetitionRepository(EfContext context) : IConstructorCompetitionRepository
{
    private readonly DbSet<ConstructorCompetition> _constructorCompetitions = context.ConstructorCompetitions;

    public async Task<ConstructorCompetition?> GetByConstructorAndChampionship(Guid constructorId, Guid championshipId) => 
        await _constructorCompetitions
            .FirstOrDefaultAsync(cc => cc.ConstructorId == constructorId && cc.ConstChampId == championshipId);

    public async Task<List<ConstructorCompetition>> GetAllConstructorCompetitions() => 
        await _constructorCompetitions.ToListAsync();

    public async Task Create(ConstructorCompetition constructorCompetition)
    {
        _constructorCompetitions.Add(constructorCompetition);
        await context.SaveChangesAsync();
    }

    public async Task Update(ConstructorCompetition constructorCompetition)
    {
        _constructorCompetitions.Update(constructorCompetition);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid constructorId, Guid championshipId)
    {
        var constCompetition = await GetByConstructorAndChampionship(constructorId, championshipId);
        if(constCompetition == null) return;
        
        _constructorCompetitions.Remove(constCompetition);
        await context.SaveChangesAsync();
    }

    public async Task<List<ConstructorCompetition>> GetByConstructorId(Guid constructorId) => 
        await _constructorCompetitions
        .Where(cc => cc.ConstructorId == constructorId)
        .ToListAsync();

    public async Task<List<ConstructorCompetition>> GetByChampionshipId(Guid champId) => await _constructorCompetitions
        .Include(cc => cc.Constructor)
        .Where(cc => cc.ConstChampId == champId)
        .ToListAsync();

    public async Task<List<Constructor?>> GetConstructorsByChampionshipId(Guid championshipId) => await _constructorCompetitions
        .Where(p => p.ConstChampId == championshipId)
        .Select(p => p.Constructor)
        .OrderBy(c => c!.Name)
        .ToListAsync();

    public async Task<bool> CheckIfExists(Guid constructorId, Guid champId) => await _constructorCompetitions
        .AnyAsync(cc => cc.ConstructorId == constructorId && cc.ConstChampId == champId);
}
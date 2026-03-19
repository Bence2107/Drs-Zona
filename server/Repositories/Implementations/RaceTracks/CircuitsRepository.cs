using Context;
using Entities.Models.RaceTracks;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.RaceTracks;

namespace Repositories.Implementations.RaceTracks;

public class CircuitsRepository(EfContext context) : ICircuitsRepository
{
    private readonly DbSet<Circuit> _circuits = context.Circuits;
    
    public async Task<Circuit?> GetCircuitById(Guid id) => 
        await _circuits.FirstOrDefaultAsync(p => p.Id == id);
    
    public async Task<List<Circuit>> GetAllCircuits() => 
        await _circuits.ToListAsync();
}
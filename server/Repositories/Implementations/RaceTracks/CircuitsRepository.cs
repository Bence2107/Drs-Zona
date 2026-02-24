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

    public async Task Create(Circuit circuit)
    {
        await _circuits.AddAsync(circuit);
        await context.SaveChangesAsync();
    }

    public async Task Update(Circuit circuit)
    {
        _circuits.Update(circuit);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var circuit = await GetCircuitById(id);
        if (circuit == null) return;
        
        _circuits.Remove(circuit);
        await context.SaveChangesAsync();
    }

    public async Task<Circuit?> GetByName(string name) => 
        await _circuits.FirstOrDefaultAsync(p => p.Name == name);
    
    public async Task<List<Circuit>> GetByLocation(string location) => 
        await _circuits.Where(p => p.Location == location).ToListAsync();

    public async Task<List<Circuit>> GetByType(string type) => 
        await _circuits.Where(p => p.Type == type).ToListAsync();

    public async Task<bool> CheckIfIdExists(Guid id) => 
        await _circuits.AnyAsync(p => p.Id == id);
}
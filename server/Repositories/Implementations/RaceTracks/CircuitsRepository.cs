using Context;
using Entities.Models.RaceTracks;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.RaceTracks;

namespace Repositories.Implementations.RaceTracks;

public class CircuitsRepository(EfContext context) : ICircuitsRepository
{
    private readonly DbSet<Circuit> _circuits = context.Circuits;
    
    public Circuit? GetCircuitById(Guid id) => _circuits.FirstOrDefault(p => p.Id == id);
    
    public List<Circuit> GetAllCircuits() =>_circuits.ToList();

    public void Create(Circuit circuit)
    {
        _circuits.Add(circuit);
        context.SaveChanges();
    }

    public void Update(Circuit circuit)
    {
        _circuits.Update(circuit);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var circuit = GetCircuitById(id);
        if(circuit == null) return;
        
        _circuits.Remove(circuit);
        context.SaveChanges();
    }

    public Circuit? GetByName(string name) => _circuits.FirstOrDefault(p => p.Name == name);
    
    public List<Circuit> GetByLocation(string location) => _circuits
        .Where(p => p.Location == location)
        .ToList();
    

    public List<Circuit> GetByType(string type) => _circuits
        .Where(p => p.Type == type)
        .ToList();

    public bool CheckIfIdExists(Guid id) =>_circuits.Any(p => p.Id == id);
}
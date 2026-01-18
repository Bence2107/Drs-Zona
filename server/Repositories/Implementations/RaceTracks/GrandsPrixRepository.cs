using Context;
using Entities.Models.RaceTracks;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.RaceTracks;

namespace Repositories.Implementations.RaceTracks;

public class GrandsPrixRepository(EfContext context) : IGrandsPrixRepository
{
    private readonly DbSet<GrandPrix> _grandsPrix = context.GrandsPrix;
    
    public GrandPrix? GetGrandPrixById(int id) => _grandsPrix.FirstOrDefault(c => c.Id == id);
    
    public List<GrandPrix> GetAllGrandPrix() => _grandsPrix.ToList();
    
    public void Create(GrandPrix grandPrix)
    {
        _grandsPrix.Add(grandPrix);
        context.SaveChanges();
    }

    public void Update(GrandPrix grandPrix)
    {
        _grandsPrix.Update(grandPrix);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var grandPrix = GetGrandPrixById(id);
        if(grandPrix == null) return;
        
        _grandsPrix.Remove(grandPrix);
        context.SaveChanges();
    }

    public GrandPrix? GetByIdWithCircuit(int id) => _grandsPrix
        .Include(c => c.Circuit)
        .FirstOrDefault(c => c.Id == id);

    public List<GrandPrix> GetBySeason(int year) => _grandsPrix
        .Where(gp => gp.SeasonYear == year)
        .ToList();
    
    public List<GrandPrix> GetByCircuitId(int circuitId) => _grandsPrix
        .Where(gp => gp.CircuitId == circuitId)
        .ToList();


    public GrandPrix? GetByRoundAndSeason(int round, int season) => _grandsPrix
        .FirstOrDefault(gp => gp.RoundNumber == round && gp.SeasonYear == season);

    public bool CheckIfIdExists(int id) => _grandsPrix.Any(c => c.Id == id);
}
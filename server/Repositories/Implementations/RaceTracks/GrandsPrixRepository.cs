using Context;
using Entities.Models.RaceTracks;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.RaceTracks;

namespace Repositories.Implementations.RaceTracks;

public class GrandsPrixRepository(EfContext context) : IGrandsPrixRepository
{
    private readonly DbSet<GrandPrix> _grandsPrix = context.GrandsPrix;
    
    public GrandPrix? GetGrandPrixById(Guid id) => _grandsPrix.FirstOrDefault(c => c.Id == id);
    
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

    public void Delete(Guid id)
    {
        var grandPrix = GetGrandPrixById(id);
        if(grandPrix == null) return;
        
        _grandsPrix.Remove(grandPrix);
        context.SaveChanges();
    }

    public GrandPrix? GetByIdWithCircuit(Guid id) => _grandsPrix
        .Include(c => c.Circuit)
        .FirstOrDefault(c => c.Id == id);

    public GrandPrix? GetWithAll(Guid id) => _grandsPrix
        .Include(c => c.Circuit)
        .Include(c => c.Series)
        .FirstOrDefault(c => c.Id == id);

    public List<GrandPrix> GetBySeason(int year) => _grandsPrix
        .Where(gp => gp.SeasonYear == year)
        .ToList();
    
    public List<GrandPrix> GetByCircuitId(Guid circuitId) => _grandsPrix
        .Where(gp => gp.CircuitId == circuitId)
        .ToList();

    public List<GrandPrix> GetBySeriesAndYear(Guid seriesId, int year) => _grandsPrix
        .Where(gp => gp.SeriesId == seriesId && gp.SeasonYear == year)
        .ToList();
    
    public GrandPrix? GetByRoundAndSeason(int round, int season) => _grandsPrix
        .FirstOrDefault(gp => gp.RoundNumber == round && gp.SeasonYear == season);

    public bool CheckIfIdExists(Guid id) => _grandsPrix.Any(c => c.Id == id);
}
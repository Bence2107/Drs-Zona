using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ResultsRepository(EfContext context) : IResultsRepository
{
    private readonly DbSet<Result> _results = context.Results;

    public IQueryable<Result> GetQueryable()
    {
        return _results.AsNoTracking();
    }

    public Result? GetResultById(Guid id) => _results.FirstOrDefault(r => r.Id == id);

    public List<Result> GetAllResults() => _results.ToList();

    public void Create(Result result)
    {
        _results.Add(result);
        context.SaveChanges();
    }

    public void Update(Result result)
    {
        _results.Update(result);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var result = GetResultById(id);
        if(result == null) return;
        
        _results.Remove(result);
        context.SaveChanges();
    }
    
    public Result? GetResultWithAll(Guid id) => _results
        .Include(r => r.Driver)
        .Include(r => r.Constructor)
        .Include(r => r.DriversChampionship)
        .Include(r => r.ConsChampionship)
        .FirstOrDefault(r => r.Id == id);

    public List<Result> GetByGrandPrixId(Guid grandPrixId) => _results
        .Where(r => r.GrandPrixId == grandPrixId)
        .ToList();

    public List<Result> GetByDriverId(Guid driverId) => _results
        .Where(r => r.DriverId == driverId)
        .ToList();

    public List<Result> GetByConstructorId(Guid constructorId) => _results
        .Where(r => r.ConstructorId == constructorId)
        .ToList();

    public List<Result> GetBySession(Guid grandPrixId, string session) => _results
        .Include(r => r.GrandPrix)
        .Include(r => r.Driver)
        .Include(r => r.Constructor)
        .Where(r => r.GrandPrixId == grandPrixId && r.Session == session)
        .ToList();

    public List<Result> GetBySession(string session) => _results
        .Where(r => r.Session == session)
        .ToList();
    
    public List<Result> GetByDriversChampionshipId(Guid championshipId) => _results
        .Include(r => r.Driver)
        .Include(r => r.Constructor)
        .Where(r=> r.DriversChampId == championshipId)
        .ToList();
    
    public List<Result> GetByConstructorsChampionshipId(Guid championshipId) => _results
        .Include(r => r.Constructor)
        .Where(r=> r.ConsChampId == championshipId)
        .ToList();

    public List<string> GetAvailableSessionsByGrandPrixId(Guid grandPrixId)
    {
        return _results
            .AsNoTracking()
            .Where(r => r.GrandPrixId == grandPrixId)
            .Select(r => r.Session)
            .Distinct() 
            .OrderBy(s => s) 
            .ToList();
    }
    
    public bool CheckIfIdExists(Guid id) => _results.Any(d => d.Id == id);
}
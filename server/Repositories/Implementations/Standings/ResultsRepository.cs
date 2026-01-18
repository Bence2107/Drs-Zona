using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ResultsRepository(EfContext context) : IResultsRepository
{
    private readonly DbSet<Result> _results = context.Results;
    
    public Result? GetResultById(int id) => _results.FirstOrDefault(r => r.Id == id);

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

    public void Delete(int id)
    {
        var result = GetResultById(id);
        if(result == null) return;
        
        _results.Remove(result);
        context.SaveChanges();
    }
    
    public Result? GetResultWithAll(int id) => _results
        .Include(r => r.Driver)
        .Include(r => r.Constructor)
        .Include(r => r.DriversChampionship)
        .Include(r => r.ConsChampionship)
        .FirstOrDefault(r => r.Id == id);

    public List<Result> GetByGrandPrixId(int grandPrixId) => _results
        .Where(r => r.GrandPrixId == grandPrixId)
        .ToList();

    public List<Result> GetByDriverId(int driverId) => _results
        .Where(r => r.DriverId == driverId)
        .ToList();

    public List<Result> GetByConstructorId(int constructorId) => _results
        .Where(r => r.ConstructorId == constructorId)
        .ToList();

    public List<Result> GetBySession(string session) => _results
        .Where(r => r.Session == session)
        .ToList();

    public bool CheckIfIdExists(int id) => _results.Any(d => d.Id == id);
}
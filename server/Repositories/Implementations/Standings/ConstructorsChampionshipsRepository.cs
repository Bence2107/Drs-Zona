using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ConstructorsChampionshipsRepository(EfContext context) : IConstructorsChampionshipsRepository
{
    private readonly DbSet<ConstructorsChampionship> _constructorsChampionships = context.ConstructorsChampionships;

    public ConstructorsChampionship? GetAllConstructorsChampionshipById(int id) => _constructorsChampionships
        .FirstOrDefault(cc => cc.Id == id);

    public List<ConstructorsChampionship> GetAllConstructorsChampionships() => _constructorsChampionships.ToList();

    public void Create(ConstructorsChampionship constructorsChampionship)
    {
        _constructorsChampionships.Add(constructorsChampionship);
        context.SaveChanges();
    }

    public void Update(ConstructorsChampionship constructorsChampionship)
    {
        _constructorsChampionships.Update(constructorsChampionship);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var constructorsChampionship = GetAllConstructorsChampionshipById(id);
        if (constructorsChampionship == null) return;

        _constructorsChampionships.Remove(constructorsChampionship);
        context.SaveChanges();
    }

    public ConstructorsChampionship? GetByIdWithSeries(int id) => _constructorsChampionships
        .Include(cc => cc.Series)
        .FirstOrDefault(cc => cc.Id == id);

    public List<ConstructorsChampionship> GetBySeason(string season) => _constructorsChampionships
        .Where(cc => cc.Season == season)
        .ToList();

    public List<ConstructorsChampionship> GetByStatus(string status) => _constructorsChampionships
        .Where(cc => cc.Status == status)
        .ToList();

    public bool CheckIfIdExists(int id) => _constructorsChampionships.Any(cc => cc.Id == id);
}
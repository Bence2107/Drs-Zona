using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriversChampionshipsRepository(EfContext context) : IDriversChampionshipsRepository
{
    private readonly DbSet<DriversChampionship> _driversChampionships = context.DriversChampionships;
    
    public DriversChampionship? GetById(int id) => _driversChampionships.FirstOrDefault(d => d.Id == id);
    
    public List<DriversChampionship> GetAll() => _driversChampionships.ToList();
    
    public void Add(DriversChampionship championship)
    {
        _driversChampionships.Add(championship);
        context.SaveChanges();
    }

    public void Modify(DriversChampionship championship)
    {
        _driversChampionships.Update(championship);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var driversChampionship = GetById(id);
        if(driversChampionship == null) return;
        
        _driversChampionships.Remove(driversChampionship);
        context.SaveChanges();
    }

    public DriversChampionship? GetByIdWithSeries(int id) => _driversChampionships
        .Include(d => d.Series)
        .FirstOrDefault(d => d.Id == id);

    public List<DriversChampionship> GetBySeason(string season) => _driversChampionships
        .Where(dc => dc.Season == season)
        .ToList();

    public List<DriversChampionship> GetByStatus(string status) => _driversChampionships.
        Where(dc => dc.Status == status)
        .ToList();

    public bool CheckIfIdExists(int id) => _driversChampionships.Any(d => d.Id == id);
}
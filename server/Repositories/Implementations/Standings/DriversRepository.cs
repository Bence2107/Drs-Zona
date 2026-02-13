using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriversRepository(EfContext context) : IDriversRepository
{
    private readonly DbSet<Driver> _drivers = context.Drivers;
    
    public Driver? GetDriverById(Guid id) => _drivers.FirstOrDefault(d => d.Id == id);

    public List<Driver> GetAllDrivers() => _drivers.ToList();
    
    public void Create(Driver driver)
    {
        _drivers.Add(driver);
        context.SaveChanges();
    }

    public void Update(Driver driver)
    {
        _drivers.Update(driver);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var driver = GetDriverById(id);
        if(driver == null) return;
        
        _drivers.Remove(driver);
        context.SaveChanges();
    }

    public List<Driver> GetByNationality(string nationality) => _drivers
        .Where(d => d.Nationality == nationality)
        .ToList();
    
    public Driver? GetByName(string name) => _drivers.FirstOrDefault(d => d.Name == name);

    public bool CheckIfIdExists(Guid id) => _drivers.Any(d => d.Id == id);
}
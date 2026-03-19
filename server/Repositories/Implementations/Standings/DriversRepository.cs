using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriversRepository(EfContext context) : IDriversRepository 
{
    private readonly DbSet<Driver> _drivers = context.Drivers;
    
    public async Task<Driver?> GetDriverById(Guid id) => await _drivers.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<List<Driver>> GetAllDrivers() => await _drivers.ToListAsync();
    
    public async Task Create(Driver driver)
    {
        await _drivers.AddAsync(driver);
        await context.SaveChangesAsync();
    }

    public async Task Update(Driver driver)
    {
        _drivers.Update(driver);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var driver = await GetDriverById(id);
        if(driver == null) return;
        
        _drivers.Remove(driver);
        await context.SaveChangesAsync();
    }
}
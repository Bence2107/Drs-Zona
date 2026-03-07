using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriverParticipationRepository(EfContext context) : IDriverParticipationRepository
{
    private readonly DbSet<DriverParticipation> _driverParticipates = context.DriverParticipates;
    
    public async Task<DriverParticipation?> GetByDriverAndChampionship(Guid driverId, Guid championshipId) => await _driverParticipates
        .FirstOrDefaultAsync(d => d.DriverId == driverId && d.DriverChampId == championshipId);

    public async Task<List<DriverParticipation>> GetAllDriverParticipation(Guid id) => await _driverParticipates.ToListAsync();

    public async Task Create(DriverParticipation driverParticipation)
    {
        await _driverParticipates.AddAsync(driverParticipation);
        await context.SaveChangesAsync();
    }

    public async Task Update(DriverParticipation driverParticipation)
    {
        _driverParticipates.Update(driverParticipation);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid driverId, Guid championshipId)
    {
        var driverParticipation = await GetByDriverAndChampionship(driverId, championshipId);
        if(driverParticipation == null) return;
        
        _driverParticipates.Remove(driverParticipation);
        await context.SaveChangesAsync();
    }

    public async Task<List<DriverParticipation>> GetByDriverId(Guid driverId) => await _driverParticipates
        .Where(d => d.DriverId == driverId)
        .ToListAsync();

    public async Task<List<DriverParticipation>> GetByChampionshipId(Guid champId) => await _driverParticipates
        .Include(cc => cc.Driver)
        .Where(d => d.DriverChampId == champId)
        .ToListAsync();
    
    public async Task<List<Driver?>> GetDriversByChampionship(Guid driversChampionshipId) => await _driverParticipates
            .Where(p => p.DriverChampId == driversChampionshipId)
            .Select(p => p.Driver)
            .ToListAsync();

    public async Task<bool> CheckIfExists(Guid driverId, Guid champId) => await _driverParticipates
        .AnyAsync(d => d.DriverId == driverId && d.DriverChampId == champId);
}
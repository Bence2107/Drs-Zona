using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriverParticipationRepository(EfContext context) : IDriverParticipationRepository
{
    private readonly DbSet<DriverParticipation> _driverParticipates = context.DriverParticipates;
    
    public DriverParticipation? GetDriverParticipationById(Guid driverId, Guid championshipId) => _driverParticipates
        .FirstOrDefault(d => d.DriverId == driverId && d.DriverChampId == championshipId);

    public List<DriverParticipation> GetAllDriverParticipation(Guid id) => _driverParticipates.ToList();

    public void Create(DriverParticipation driverParticipation)
    {
        _driverParticipates.Add(driverParticipation);
        context.SaveChanges();
    }

    public void Update(DriverParticipation driverParticipation)
    {
        _driverParticipates.Update(driverParticipation);
        context.SaveChanges();
    }

    public void Delete(Guid driverId, Guid championshipId)
    {
        var driverParticipation = GetDriverParticipationById(driverId, championshipId);
        if(driverParticipation == null) return;
        
        _driverParticipates.Remove(driverParticipation);
        context.SaveChanges();
}

    public List<DriverParticipation> GetByDriverId(Guid driverId) => _driverParticipates
        .Where(d => d.DriverId == driverId)
        .ToList();

    public List<DriverParticipation> GetByChampionshipId(Guid champId) => _driverParticipates
        .Where(d => d.DriverChampId == champId)
        .ToList();
    
    public List<Driver?> GetDriversByChampionship(Guid driversChampionshipId) => _driverParticipates
            .Where(p => p.DriverChampId == driversChampionshipId)
            .Select(p => p.Driver)
            .ToList();
    

    public bool CheckIfExists(Guid driverId, Guid champId) =>  _driverParticipates
        .Any(d => d.DriverId == driverId && d.DriverChampId == champId);
}
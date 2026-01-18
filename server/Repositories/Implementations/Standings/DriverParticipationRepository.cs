using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class DriverParticipationRepository(EfContext context) : IDriverParticipationRepository
{
    private readonly DbSet<DriverParticipation> _driverParticipates = context.DriverParticipates;
    
    public DriverParticipation? GetDriverParticipationById(int driverId, int championshipId) => _driverParticipates
        .FirstOrDefault(d => d.DriverId == driverId && d.DriverChampId == championshipId);

    public List<DriverParticipation> GetAllDriverParticipation(int id) => _driverParticipates.ToList();

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

    public void Delete(int driverId, int championshipId)
    {
        var driverParticipation = GetDriverParticipationById(driverId, championshipId);
        if(driverParticipation == null) return;
        
        _driverParticipates.Remove(driverParticipation);
        context.SaveChanges();
}

    public List<DriverParticipation> GetByDriverId(int driverId) => _driverParticipates
        .Where(d => d.DriverId == driverId)
        .ToList();

    public List<DriverParticipation> GetByChampionshipId(int champId) => _driverParticipates
        .Where(d => d.DriverChampId == champId)
        .ToList();
    

    public bool CheckIfExists(int driverId, int champId) =>  _driverParticipates
        .Any(d => d.DriverId == driverId && d.DriverChampId == champId);
}
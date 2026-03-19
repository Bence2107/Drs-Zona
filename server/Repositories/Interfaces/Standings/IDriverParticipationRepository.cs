using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriverParticipationRepository 
{
    Task<DriverParticipation?> GetByDriverAndChampionship(Guid driverId, Guid championshipId);
    Task<List<DriverParticipation>> GetByChampionshipId(Guid champId);
    Task<List<Driver?>> GetDriversByChampionship(Guid driversChampionshipId);
    Task Create(DriverParticipation driverParticipation);
    Task Delete(Guid driverId, Guid championshipId);
    Task<bool> CheckIfExists(Guid driverId, Guid champId);
}
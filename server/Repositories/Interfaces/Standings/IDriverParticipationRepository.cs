using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriverParticipationRepository
{
    Task<DriverParticipation?> GetDriverParticipationById(Guid driverId, Guid championshipId);
    Task<List<DriverParticipation>> GetAllDriverParticipation(Guid id);
    Task Create(DriverParticipation driverParticipation);
    Task Update(DriverParticipation driverParticipation);
    Task Delete(Guid driverId, Guid championshipId);
    Task<List<DriverParticipation>> GetByDriverId(Guid driverId);
    Task<List<DriverParticipation>> GetByChampionshipId(Guid champId);
    Task<List<Driver?>> GetDriversByChampionship(Guid driversChampionshipId);
    Task<bool> CheckIfExists(Guid driverId, Guid champId);
}
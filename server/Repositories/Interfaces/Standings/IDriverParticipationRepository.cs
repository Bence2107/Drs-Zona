using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriverParticipationRepository
{
    DriverParticipation? GetDriverParticipationById(Guid driverId, Guid championshipId);
    List<DriverParticipation> GetAllDriverParticipation(Guid id);
    void Create(DriverParticipation driverParticipation);
    void Update(DriverParticipation driverParticipation);
    void Delete(Guid driverId, Guid championshipId);
    List<DriverParticipation> GetByDriverId(Guid driverId);
    List<DriverParticipation> GetByChampionshipId(Guid champId);
    List<Driver?> GetDriversByChampionship(Guid driversChampionshipId);
    bool CheckIfExists(Guid driverId, Guid champId);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriverParticipationRepository
{
    DriverParticipation? GetDriverParticipationById(int driverId, int championshipId);
    List<DriverParticipation> GetAllDriverParticipation(int id);
    void Create(DriverParticipation driverParticipation);
    void Update(DriverParticipation driverParticipation);
    void Delete(int driverId, int championshipId);
    List<DriverParticipation> GetByDriverId(int driverId);
    List<DriverParticipation> GetByChampionshipId(int champId);
    bool CheckIfExists(int driverId, int champId);
}
using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversRepository
{
    Driver? GetDriverById(Guid id);
    List<Driver> GetAllDrivers();
    void Create(Driver driver);
    void Update(Driver driver);
    void Delete(Guid id);
    List<Driver> GetByNationality(string nationality);
    Driver? GetByDriverNumber(int number);
    Driver? GetByName(string name);
    bool CheckIfIdExists(Guid id);
}
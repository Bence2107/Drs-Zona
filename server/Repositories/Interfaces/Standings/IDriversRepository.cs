using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversRepository
{
    Driver? GetDriverById(int id);
    List<Driver> GetAllDrivers();
    void Create(Driver driver);
    void Update(Driver driver);
    void Delete(int id);
    List<Driver> GetByNationality(string nationality);
    Driver? GetByDriverNumber(int number);
    Driver? GetByName(string name);
    bool CheckIfIdExists(int id);
}
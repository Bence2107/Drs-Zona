using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IDriversRepository
{
    Task<Driver?> GetDriverById(Guid id);
    Task<List<Driver>> GetAllDrivers();
    Task Create(Driver driver);
    Task Update(Driver driver);
    Task Delete(Guid id);
    Task<List<Driver>> GetByNationality(string nationality);
    Task<Driver?> GetByName(string name);
    Task<bool> CheckIfIdExists(Guid id);
}
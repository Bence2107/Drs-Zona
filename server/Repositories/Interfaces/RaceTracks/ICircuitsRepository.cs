using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface ICircuitsRepository
{
    Task<Circuit?> GetCircuitById(Guid id);
    Task<List<Circuit>> GetAllCircuits();
    Task Create(Circuit circuit);
    Task Update(Circuit circuit);
    Task Delete(Guid id);
    Task<Circuit?> GetByName(string name);
    Task<List<Circuit>> GetByLocation(string location);
    Task<List<Circuit>> GetByType(string type);
    Task<bool> CheckIfIdExists(Guid id);
}
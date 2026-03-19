using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface ICircuitsRepository
{
    Task<Circuit?> GetCircuitById(Guid id);
    Task<List<Circuit>> GetAllCircuits();
}
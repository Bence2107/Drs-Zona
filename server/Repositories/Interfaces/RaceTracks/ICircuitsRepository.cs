using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface ICircuitsRepository
{
    Circuit? GetCircuitById(Guid id);
    List<Circuit> GetAllCircuits();
    void Create(Circuit circuit);
    void Update(Circuit circuit);
    void Delete(Guid id);
    Circuit? GetByName(string name);
    List<Circuit> GetByLocation(string location);
    List<Circuit> GetByType(string type);
    bool CheckIfIdExists(Guid id);
}
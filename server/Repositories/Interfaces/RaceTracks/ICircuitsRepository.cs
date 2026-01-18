using Entities.Models.RaceTracks;

namespace Repositories.Interfaces.RaceTracks;

public interface ICircuitsRepository
{
    Circuit? GetCircuitById(int id);
    List<Circuit> GetAllCircuits();
    void Create(Circuit circuit);
    void Update(Circuit circuit);
    void Delete(int id);
    Circuit? GetByName(string name);
    List<Circuit> GetByLocation(string location);
    List<Circuit> GetByType(string type);
    bool CheckIfIdExists(int id);
}
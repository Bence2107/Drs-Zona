using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IContractsRepository
{
    Contract? GetContractById(int id);
    Contract? GetContractByDriverAndTConstructorId(int driverId, int constructorId);
    List<Contract> GetContracts();
    void Create(Contract contract);
    void Update(Contract contract);
    void Delete(int id);
    Contract? GetByIdWithAll(int id);
    List<Contract> GetByDriverId(int driverId);
    List<Contract> GetByTeamId(int teamId);
    bool CheckIfIdExists(int id);
}
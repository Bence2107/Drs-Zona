using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IContractsRepository
{
    Contract? GetContractById(Guid id);
    Contract? GetContractByDriverAndTConstructorId(Guid driverId, Guid constructorId);
    List<Contract> GetContracts();
    void Create(Contract contract);
    void Update(Contract contract);
    void Delete(Guid id);
    Contract? GetByIdWithAll(Guid id);
    List<Contract> GetByDriverId(Guid driverId);
    List<Contract> GetByTeamId(Guid teamId);
    bool CheckIfIdExists(Guid id);
}
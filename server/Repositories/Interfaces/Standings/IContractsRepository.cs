using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IContractsRepository
{
    Task<Contract?> GetContractById(Guid id);
    Task<Contract?> GetContractByDriverAndTConstructorId(Guid driverId, Guid constructorId);
    Task<List<Contract>> GetContracts();
    Task Create(Contract contract);
    Task Update(Contract contract);
    Task Delete(Guid id);
    Task<Contract?> GetByIdWithAll(Guid id);
    Task<List<Contract>> GetByDriverId(Guid driverId);
    Task<List<Contract>> GetByTeamId(Guid teamId);
    Task<bool> CheckIfIdExists(Guid id);
}
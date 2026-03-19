using Entities.Models.Standings;

namespace Repositories.Interfaces.Standings;

public interface IContractsRepository
{
    Task<Contract?> GetContractById(Guid id);
    Task<List<Contract>> GetAllWithAll();
    Task<List<Contract>> GetByDriverId(Guid driverId);
    Task<List<Contract>> GetByTeamId(Guid teamId);
    Task Create(Contract contract);
    Task Update(Contract contract);
    Task Delete(Guid id);
    Task<bool> CheckIfIdExists(Guid id);
}
using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ContractsRepository(EfContext context) : IContractsRepository
{
    private readonly DbSet<Contract> _contracts = context.Contracts;
    
    public Contract? GetContractById(Guid id) => _contracts.FirstOrDefault(c => c.Id == id);
    public Contract? GetContractByDriverAndTConstructorId(Guid driverId, Guid constructorId)=> _contracts 
        .FirstOrDefault(c => c.DriverId == driverId && c.ConstructorId == constructorId);

    public List<Contract> GetContracts() => _contracts.ToList();

    public void Create(Contract contract)
    {
        _contracts.Add(contract);
        context.SaveChanges();
    }

    public void Update(Contract contract)
    {
        _contracts.Update(contract);
        context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var contract = GetContractById(id);
        if(contract == null) return;
        
        _contracts.Remove(contract);
        context.SaveChanges();
    }

    public Contract? GetByIdWithAll(Guid id) => _contracts
        .Include(c => c.Driver)
        .Include(c => c.Constructor)
        .FirstOrDefault(c => c.Id == id);

    public List<Contract> GetByDriverId(Guid driverId) => _contracts
        .Where(c => c.DriverId == driverId)
        .ToList();

    public List<Contract> GetByTeamId(Guid teamId) => _contracts
        .Include(c => c.Driver)
        .Where(c => c.ConstructorId == teamId)
        .ToList();

    public bool CheckIfIdExists(Guid id) => _contracts.Any(c => c.Id == id);
}
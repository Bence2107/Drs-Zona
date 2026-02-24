using Context;
using Entities.Models.Standings;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces.Standings;

namespace Repositories.Implementations.Standings;

public class ContractsRepository(EfContext context) : IContractsRepository
{
    private readonly DbSet<Contract> _contracts = context.Contracts;
    
    public async Task<Contract?> GetContractById(Guid id) => await _contracts.FirstOrDefaultAsync(c => c.Id == id);
    
    public async Task<Contract?> GetContractByDriverAndTConstructorId(Guid driverId, Guid constructorId) => await _contracts 
        .FirstOrDefaultAsync(c => c.DriverId == driverId && c.ConstructorId == constructorId);

    public async Task<List<Contract>> GetContracts() => await _contracts.ToListAsync();

    public async Task Create(Contract contract)
    {
        await _contracts.AddAsync(contract);
        await context.SaveChangesAsync();
    }

    public async Task Update(Contract contract)
    {
        _contracts.Update(contract);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var contract = await GetContractById(id);
        if(contract == null) return;
        
        _contracts.Remove(contract);
        await context.SaveChangesAsync();
    }

    public async Task<Contract?> GetByIdWithAll(Guid id) => await _contracts
        .Include(c => c.Driver)
        .Include(c => c.Constructor)
        .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Contract>> GetByDriverId(Guid driverId) => await _contracts
        .Where(c => c.DriverId == driverId)
        .ToListAsync();

    public async Task<List<Contract>> GetByTeamId(Guid teamId) => await _contracts
        .Include(c => c.Driver)
        .Where(c => c.ConstructorId == teamId)
        .ToListAsync();

    public async Task<bool> CheckIfIdExists(Guid id) => await _contracts.AnyAsync(c => c.Id == id);
}
using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Types;

namespace Services.Implementations;

public class ContractsService(
    IContractsRepository contractsRepo
) : IContractsService
{
    public async Task<ResponseResult<List<ContractListDto>>> GetAllContracts()
    {
        var contracts = await contractsRepo.GetAllWithAll();
        var dto = contracts.Select(c => new ContractListDto(
            Id: c.Id,
            DriverId: c.DriverId,
            DriverName: c.Driver?.Name ?? "Üres",
            TeamId: c.ConstructorId,
            TeamName: c.Constructor?.Name ?? "Üres"
        ))
            .OrderBy(c => c.TeamName)
            .ToList();
        return ResponseResult<List<ContractListDto>>.Success(dto);
    }
    
    public async Task<ResponseResult<bool>> Create(ContractCreateDto dto)
    {
        var contract = new Contract
        {
            DriverId = dto.DriverId,
            ConstructorId = dto.TeamId
        };

        await contractsRepo.Create(contract);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Update(Guid id, Guid driverId, Guid teamId)
    {
        var contract = await contractsRepo.GetContractById(id);
        if (contract == null) return ResponseResult<bool>.Failure("Contract not found");

        contract.DriverId = driverId;
        contract.ConstructorId = teamId;

        await contractsRepo.Update(contract);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Delete(Guid id)
    {
        var exists = await contractsRepo.CheckIfIdExists(id);
        if (!exists) return ResponseResult<bool>.Failure("Contract not found");

        await contractsRepo.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
}
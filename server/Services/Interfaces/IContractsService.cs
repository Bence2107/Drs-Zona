using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IContractsService
{
    Task<ResponseResult<List<ContractListDto>>> GetAllContracts();
    Task<ResponseResult<bool>> CreateContract(ContractCreateDto dto);
    Task<ResponseResult<bool>> UpdateContract(Guid id, Guid driverId, Guid teamId);
    Task<ResponseResult<bool>> DeleteContract(Guid id);
}
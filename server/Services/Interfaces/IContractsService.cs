using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IContractsService
{
    Task<ResponseResult<List<ContractListDto>>> GetAllContracts();
    Task<ResponseResult<bool>> Create(ContractCreateDto dto);
    Task<ResponseResult<bool>> Update(Guid id, Guid driverId, Guid teamId);
    Task<ResponseResult<bool>> Delete(Guid id);
}
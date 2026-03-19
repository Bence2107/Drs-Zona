using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IDriverService 
{
    Task<ResponseResult<DriverDetailDto>> GetDriverById(Guid id);
    Task<ResponseResult<List<DriverListDto>>> GetAllDrivers();
    Task<ResponseResult<bool>> Create(DriverCreateDto dto);
    Task<ResponseResult<bool>> Update(DriverUpdateDto dto);
    Task<ResponseResult<bool>> Delete(Guid id);
}
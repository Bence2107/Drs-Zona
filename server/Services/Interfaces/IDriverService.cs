using DTOs.Standings;

namespace Services.Interfaces;

public interface IDriverService
{
    Task<ResponseResult<DriverDetailDto>> GetDriverById(Guid id);
    Task<ResponseResult<List<DriverListDto>>> GetAllDrivers();
    Task<ResponseResult<List<DriverListDto>>> ListAllDriversByChampionships(Guid championshipId);
    Task<ResponseResult<bool>> CreateDriver(DriverCreateDto dto);
    Task<ResponseResult<bool>> UpdateDriver(DriverUpdateDto dto);
    Task<ResponseResult<bool>> DeleteDriver(Guid id);
}
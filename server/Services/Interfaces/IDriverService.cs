using DTOs.Standings;

namespace Services.Interfaces;

public interface IDriverService
{
    ResponseResult<DriverDetailDto> GetDriverById(int id);
    ResponseResult<List<DriverListDto>> ListAllDriversByChampionships(int championshipId);
    ResponseResult<bool> CreateDriver(DriverCreateDto dto);
    ResponseResult<bool> UpdateDriver(DriverUpdateDto dto);
    ResponseResult<bool> DeleteDriver(int id);
}
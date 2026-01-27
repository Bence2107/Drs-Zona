using DTOs.Standings;

namespace Services.Interfaces;

public interface IDriverService
{
    ResponseResult<DriverDetailDto> GetDriverById(Guid id);
    ResponseResult<List<DriverListDto>> ListAllDriversByChampionships(Guid championshipId);
    ResponseResult<bool> CreateDriver(DriverCreateDto dto);
    ResponseResult<bool> UpdateDriver(DriverUpdateDto dto);
    ResponseResult<bool> DeleteDriver(Guid id);
}
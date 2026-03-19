using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IChampionshipService 
{
    Task<ResponseResult<List<ChampionshipRowDto>>> GetAllChampionshipsBySeries(Guid seriesId);
    Task<ResponseResult<List<ConstructorLookUpDto>>> GetConstructorsBySeason(Guid driversChampId);
    Task<ResponseResult<List<DriverLookUpDto>>> GetDriversBySeason(Guid driversChampId);
    Task<ResponseResult<List<GrandPrixLookupDto>>> GetGrandsPrixByChampionship(Guid driverChampId);
    Task<ResponseResult<ParticipationListDto>> GetParticipations(Guid driversChampId, Guid constructorsChampId);
    Task<ResponseResult<List<YearLookupDto>>> GetSeasonsBySeries(Guid seriesId);
    Task<ResponseResult<bool>> CreateChampionship(ChampionshipCreateDto createDto);
    Task<ResponseResult<bool>> CreateParticipations(ParticipationAddDto dto);
    Task<ResponseResult<bool>> UpdateChampionshipStatus(Guid driversChampId, Guid constructorsChampId, string status);
    Task<ResponseResult<bool>> DeleteDriverParticipation(Guid driverId, Guid driversChampId);
    Task<ResponseResult<bool>> DeleteConstructorCompetition(Guid constructorId, Guid constructorsChampId);
}
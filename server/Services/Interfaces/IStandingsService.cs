using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IStandingsService
{
    Task<ResponseResult<List<SeriesLookupDto>>> GetAllSeries();
    Task<ResponseResult<ConstructorStandingsDto>> GetConstructorStandings(Guid constructorsChampionId);
    Task<ResponseResult<List<ConstructorSeasonResultDto>>> GetConstructorResultsBySeason(Guid constructorId, Guid constructorChampId);
    Task<ResponseResult<List<DriverSeasonResultDto>>> GetDriverResultsBySeason(Guid driverId, Guid driverChampId);
    Task<ResponseResult<DriverStandingsDto>> GetDriverStandings(Guid driverChampId);
    Task<ResponseResult<GrandPrixChampionshipContextDto>> GetGrandPrixContext(Guid grandPrixId);
    Task<ResponseResult<GrandPrixResultsDto>> GetGrandPrixResults(Guid grandPrixId, string session);
    Task<ResponseResult<List<SeasonOverviewDto>>> GetSeasonOverview(Guid driverChampId);
    Task<ResponseResult<SessionEditDto>> GetSessionForEdit(Guid grandPrixId, string session);
    Task<ResponseResult<List<string>>> GetSessionsByGrandPrix(Guid grandPrixId);
    Task<ResponseResult<bool>> InsertResults(BatchResultCreateDto dto);
    Task<ResponseResult<bool>> SaveSessionResults(BatchResultCreateDto dto);
    Task<ResponseResult<bool>> UpdateSingleResult(SingleResultUpdateDto dto);
    Task<ResponseResult<bool>> RecalculateSession(Guid grandPrixId, string session);
}
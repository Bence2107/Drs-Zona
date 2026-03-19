using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface IStandingsService
{
    Task<ResponseResult<List<SeriesLookupDto>>> GetAllSeries();
    Task<ResponseResult<ParticipationListDto>> GetParticipations(Guid driversChampId, Guid constructorsChampId);
    Task<ResponseResult<List<ChampionshipRowDto>>> GetAllChampionshipsBySeries(Guid seriesId);
    Task<ResponseResult<List<YearLookupDto>>> GetSeasonsBySeries(Guid seriesId);
    Task<ResponseResult<List<DriverLookUpDto>>> GetDriversBySeason(Guid driversChampId);
    Task<ResponseResult<List<ConstructorLookUpDto>>> GetConstructorsBySeason(Guid driversChampId);
    Task<ResponseResult<List<GrandPrixLookupDto>>> GetGrandsPrixByChampionship(Guid driverChampId);
    Task<ResponseResult<List<string>>> GetSessionsByGrandPrix(Guid grandPrixId);
    Task<ResponseResult<DriverStandingsDto>> GetDriverStandings(Guid driverChampId);
    Task<ResponseResult<ConstructorStandingsDto>> GetConstructorStandings(Guid constructorsChampionId);
    Task<ResponseResult<GrandPrixResultsDto>> GetGrandPrixResults(Guid grandPrixId, string session);
    
    Task<ResponseResult<List<SeasonOverviewDto>>> GetSeasonOverview(Guid driverChampId);
    Task<ResponseResult<List<DriverSeasonResultDto>>> GetDriverResultsBySeason(Guid driverId, Guid driverChampId);
    Task<ResponseResult<List<ConstructorSeasonResultDto>>> GetConstructorResultsBySeason(Guid constructorId, Guid constructorChampId);
    Task<ResponseResult<bool>> CreateChampionship(ChampionshipCreateDto createDto);
    Task<ResponseResult<bool>> UpdateChampionshipStatus(Guid driversChampId, Guid constructorsChampId, string status);
    Task<ResponseResult<bool>> AddParticipations(ParticipationAddDto dto);
    Task<ResponseResult<bool>> RemoveDriverParticipation(Guid driverId, Guid driversChampId);
    Task<ResponseResult<bool>> RemoveConstructorCompetition(Guid constructorId, Guid constructorsChampId);
    Task<ResponseResult<bool>> InsertResults(BatchResultCreateDto dto);
    Task<ResponseResult<bool>> SaveSessionResults(BatchResultCreateDto dto);
    Task<ResponseResult<GrandPrixChampionshipContextDto>> GetGrandPrixContext(Guid grandPrixId);
    Task<ResponseResult<SessionEditDto>> GetSessionForEdit(Guid grandPrixId, string session);
    Task<ResponseResult<bool>> UpdateSingleResult(SingleResultUpdateDto dto);
    Task<ResponseResult<bool>> RecalculateSession(Guid grandPrixId, string session);
}
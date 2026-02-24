using DTOs.Standings;

namespace Services.Interfaces;

public interface IStandingsService
{
    Task<ResponseResult<DefaultFiltersDto>> GetDefaultFilters();
    Task<ResponseResult<List<SeriesLookupDto>>> GetAllSeries();
    Task<ResponseResult<List<YearLookupDto>>> GetSeasonsBySeries(Guid seriesId);
    Task<ResponseResult<List<GrandPrixLookupDto>>> GetGrandsPrixByChampionship(Guid driverChampId);
    Task<ResponseResult<List<string>>> GetSessionsByGrandPrix(Guid grandPrixId);
    Task<ResponseResult<DriverStandingsDto>> GetDriverStandings(Guid driverChampId);
    Task<ResponseResult<ConstructorStandingsDto>> GetConstructorStandings(Guid constructorsChampionId);
    Task<ResponseResult<GrandPrixResultsDto>> GetGrandPrixResults(Guid grandPrixId, string session);
    Task<ResponseResult<List<DriverSeasonResultDto>>> GetDriverResultsBySeason(Guid driverId, Guid driverChampId);
    Task<ResponseResult<List<ConstructorSeasonResultDto>>> GetConstructorResultsBySeason(Guid constructorId, Guid constructorChampId);
    Task<ResponseResult<List<SeasonOverviewDto>>> GetSeasonOverview(Guid driverChampId);
}
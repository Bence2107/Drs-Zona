using DTOs.Standings;

namespace Services.Interfaces;

public interface IStandingsService
{
    ResponseResult<DefaultFiltersDto> GetDefaultFilters();
    
    ResponseResult<List<SeriesLookupDto>> GetAllSeries();

    ResponseResult<List<YearLookupDto>> GetSeasonsBySeries(Guid seriesId);

    ResponseResult<List<GrandPrixLookupDto>> GetGrandsPrixByChampionship(Guid driverChampId);

    ResponseResult<List<string>> GetSessionsByGrandPrix(Guid grandPrixId);
    
    ResponseResult<DriverStandingsDto> GetDriverStandings(Guid driverChampId);
    
    ResponseResult<ConstructorStandingsDto> GetConstructorStandings(Guid constructorsChampionId);

    ResponseResult<GrandPrixResultsDto> GetGrandPrixResults(Guid grandPrixId, string session);
}
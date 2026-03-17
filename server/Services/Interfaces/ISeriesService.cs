using DTOs.Standings;
using Services.Types;

namespace Services.Interfaces;

public interface ISeriesService
{
    Task<ResponseResult<SeriesDetailDto>> GetSeriesById(Guid seriesId);
    Task<ResponseResult<SeriesDetailDto>> GetSeriesByName(string name);
    Task<ResponseResult<List<SeriesListDto>>> ListSeries();
}
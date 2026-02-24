using DTOs.Standings;

namespace Services.Interfaces;

public interface ISeriesService
{
    Task<ResponseResult<SeriesDetailDto>> GetSeriesById(Guid seriesId);
    Task<ResponseResult<SeriesDetailDto>> GetSeriesByName(string name);
    Task<ResponseResult<List<SeriesListDto>>> ListSeries();
    Task<ResponseResult<bool>> CreateSeries(SeriesCreateDto dto);
    Task<ResponseResult<bool>> Update(SeriesUpdateDto dto);
    Task<ResponseResult<bool>> Delete(Guid id);
}
using DTOs.Standings;

namespace Services.Interfaces;

public interface ISeriesService
{
    ResponseResult<SeriesDetailDto> GetSeriesById(Guid seriesId);
    ResponseResult<SeriesDetailDto> GetSeriesByName(string name);
    ResponseResult<List<SeriesListDto>> ListSeries();
    ResponseResult<bool> CreateSeries(SeriesCreateDto dto);
    ResponseResult<bool> Update(SeriesUpdateDto dto);
    ResponseResult<bool> Delete(Guid id);
}
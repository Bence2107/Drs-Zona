using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class SeriesService(
    ISeriesRepository seriesRepo,
    IDriversChampionshipsRepository driversChampRepo,
    IConstructorsChampionshipsRepository constructorsChampRepo
) : ISeriesService
{
    public ResponseResult<SeriesDetailDto> GetSeriesById(Guid seriesId)
    {
        var series = seriesRepo.GetSeriesById(seriesId);
        if (series == null) return ResponseResult<SeriesDetailDto>.Failure("Series not exist");

        var driverSeasons = driversChampRepo.GetBySeriesId(seriesId)
            .Select(dc => dc.Season);

        var constructorSeasons = constructorsChampRepo.GetBySeriesId(seriesId)
            .Select(cc => cc.Season);

        var availableSeasons = driverSeasons
            .Union(constructorSeasons)
            .OrderByDescending(s => s)
            .ToList();

        return ResponseResult<SeriesDetailDto>.Success(new SeriesDetailDto(
            Id: series.Id,
            Name: series.Name,
            Description: series.Description,
            GoverningBody: series.GoverningBody,
            FirstYear: series.FirstYear,
            LastYear: series.LastYear,
            AvailableSeasons: availableSeasons
        ));
    }

    public ResponseResult<SeriesDetailDto> GetSeriesByName(string name)
    {
        var series = seriesRepo.GetByName(name);
        if (series == null) return ResponseResult<SeriesDetailDto>.Failure("Series not exist");

        var driverSeasons = driversChampRepo.GetBySeriesId(series.Id)
            .Select(dc => dc.Season);

        var constructorSeasons = constructorsChampRepo.GetBySeriesId(series.Id)
            .Select(cc => cc.Season);

        var availableSeasons = driverSeasons
            .Union(constructorSeasons)
            .OrderByDescending(s => s)
            .ToList();

        return ResponseResult<SeriesDetailDto>.Success(new SeriesDetailDto(
            Id: series.Id,
            Name: series.Name,
            Description: series.Description,
            GoverningBody: series.GoverningBody,
            FirstYear: series.FirstYear,
            LastYear: series.LastYear,
            AvailableSeasons: availableSeasons
        ));
    }

    public ResponseResult<List<SeriesListDto>> ListSeries()
    {
        var series = seriesRepo.GetAllSeries();
        var dto = series.Select(d => new SeriesListDto(
            Id: d.Id,
            Name: d.Name)
        ).ToList();

        return ResponseResult<List<SeriesListDto>>.Success(dto);
    }

    public ResponseResult<bool> CreateSeries(SeriesCreateDto dto)
    {
        var nameExits = seriesRepo.GetByName(dto.Name);
        if (nameExits is not null)
        {
            return ResponseResult<bool>.Failure(nameof(dto.Name), "Series with this name already exist");
        }

        var series = new Series
        {
            Name = dto.Name,
            Description = dto.Description,
            GoverningBody = dto.GoverningBody,
            FirstYear = dto.FirstYear,
            LastYear = dto.LastYear
        };

        seriesRepo.Create(series);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> Update(SeriesUpdateDto dto)
    {
        var editable = seriesRepo.GetSeriesById(dto.Id);
        if (editable is null) return ResponseResult<bool>.Failure("Series not exist");

        var series = new Series
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            GoverningBody = dto.GoverningBody,
            FirstYear = dto.FirstYear,
            LastYear = dto.LastYear
        };

        seriesRepo.Create(series);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> Delete(Guid id)
    {
        var series = seriesRepo.GetSeriesById(id);
        if (series is null) return ResponseResult<bool>.Failure("Series not exist");

        seriesRepo.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
}
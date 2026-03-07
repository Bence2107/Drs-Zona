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
    public async Task<ResponseResult<SeriesDetailDto>> GetSeriesById(Guid seriesId)
    {
        var series = await seriesRepo.GetSeriesById(seriesId);
        if (series == null) return ResponseResult<SeriesDetailDto>.Failure("Series not exist");

        var driverChamps = await driversChampRepo.GetBySeriesId(seriesId);
        var driverSeasons = driverChamps.Select(dc => dc.Season);

        var constructorChamps = await constructorsChampRepo.GetBySeriesId(seriesId);
        var constructorSeasons = constructorChamps.Select(cc => cc.Season);

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

    public async Task<ResponseResult<SeriesDetailDto>> GetSeriesByName(string name)
    {
        var series = await seriesRepo.GetByName(name);
        if (series == null) return ResponseResult<SeriesDetailDto>.Failure("Series not exist");

        var driverChamps = await driversChampRepo.GetBySeriesId(series.Id);
        var driverSeasons = driverChamps.Select(dc => dc.Season);

        var constructorChamps = await constructorsChampRepo.GetBySeriesId(series.Id);
        var constructorSeasons = constructorChamps.Select(cc => cc.Season);

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

    public async Task<ResponseResult<List<SeriesListDto>>> ListSeries()
    {
        var series = await seriesRepo.GetAllSeries();
        var dto = series.Select(d => new SeriesListDto(
            Id: d.Id,
            Name: d.Name)
        ).ToList();

        return ResponseResult<List<SeriesListDto>>.Success(dto);
    }

    public async Task<ResponseResult<bool>> CreateSeries(SeriesCreateDto dto)
    {
        var nameExits = await seriesRepo.GetByName(dto.Name);
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
            LastYear = dto.LastYear,
            PointSystem = dto.PointSystem
        };

        await seriesRepo.Create(series);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Update(SeriesUpdateDto dto)
    {
        var editable = await seriesRepo.GetSeriesById(dto.Id);
        if (editable is null) return ResponseResult<bool>.Failure("Series not exist");

        var series = new Series
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            GoverningBody = dto.GoverningBody,
            FirstYear = dto.FirstYear,
            LastYear = dto.LastYear,
            PointSystem = dto.PointSystem
        };

        await seriesRepo.Create(series);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Delete(Guid id)
    {
        var series = await seriesRepo.GetSeriesById(id);
        if (series is null) return ResponseResult<bool>.Failure("Series not exist");

        await seriesRepo.Delete(id);
        return ResponseResult<bool>.Success(true);
    }
}
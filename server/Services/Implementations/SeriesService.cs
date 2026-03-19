using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Types;

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
            ShortName: series.ShortName,
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
            ShortName: series.ShortName,
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
                Name: d.Name,
                ShortName: d.ShortName
            ))
            .OrderBy(s => s.ShortName)
            .ToList();

        return ResponseResult<List<SeriesListDto>>.Success(dto);
    }
}
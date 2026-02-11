using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class StandingsService (
    IDriversChampionshipsRepository driverChampRepo,
    IConstructorsChampionshipsRepository constructorChampRepo,
    IResultsRepository resultsRepo,
    IGrandsPrixRepository grandsPrixRepo,
    ISeriesRepository seriesRepo
)
: IStandingsService
{
    public ResponseResult<DefaultFiltersDto> GetDefaultFilters()
    {
        var series = seriesRepo.GetAllSeries().FirstOrDefault();
        if (series == null) return ResponseResult<DefaultFiltersDto>.Failure("Nincs elérhető széria");

        var latestChamp = driverChampRepo.GetBySeriesId(series.Id)
            .OrderByDescending(c => c.Season)
            .FirstOrDefault();
        if (latestChamp == null) return ResponseResult<DefaultFiltersDto>.Failure("Nincs elérhető szezon");

        var latestGp = grandsPrixRepo.GetBySeriesAndYear(series.Id, int.Parse(latestChamp.Season))
            .OrderByDescending(g => g.RoundNumber)
            .FirstOrDefault();
        if (latestGp == null) return ResponseResult<DefaultFiltersDto>.Failure("Nincs elérhető nagydíj");

        var sessions = resultsRepo.GetAvailableSessionsByGrandPrixId(latestGp.Id);
        var defaultSession = sessions.Contains("Race") ? "Race" : sessions.FirstOrDefault() ?? "Race";

        return ResponseResult<DefaultFiltersDto>.Success(new DefaultFiltersDto(
            series.Id,
            latestChamp.Id,
            latestGp.Id,
            defaultSession
        ));
    }
    
    public ResponseResult<List<SeriesLookupDto>> GetAllSeries()
    {
        var series = seriesRepo.GetAllSeries();
        var dtoS = series.Select(s => new SeriesLookupDto(s.Id, s.Name)).ToList();
        return ResponseResult<List<SeriesLookupDto>>.Success(dtoS);
    }

    public ResponseResult<List<YearLookupDto>> GetSeasonsBySeries(Guid seriesId)
    {
        var championships = driverChampRepo.GetBySeriesId(seriesId); 
        var dtoS = championships
            .Select(c => new YearLookupDto(c.Season.ToString(), c.Id))
            .OrderByDescending(y => y.Season)
            .ToList();
        return ResponseResult<List<YearLookupDto>>.Success(dtoS);    }

    public ResponseResult<List<GrandPrixLookupDto>> GetGrandsPrixByChampionship(Guid driverChampId)
    {
        var champ = driverChampRepo.GetById(driverChampId);
        if (champ == null) return ResponseResult<List<GrandPrixLookupDto>>.Failure("Bajnokság nem található");
        
        var gps = grandsPrixRepo.GetBySeriesAndYear(champ.SeriesId, int.Parse(champ.Season));
        var dtoS = gps.Select(g => new GrandPrixLookupDto(g.Id, g.Name)).ToList();
        return ResponseResult<List<GrandPrixLookupDto>>.Success(dtoS);
    }
    
    public ResponseResult<List<string>> GetSessionsByGrandPrix(Guid grandPrixId)
    {
        var sessions = resultsRepo.GetAvailableSessionsByGrandPrixId(grandPrixId);
        return ResponseResult<List<string>>.Success(sessions);
    }

    public ResponseResult<DriverStandingsDto> GetDriverStandings(Guid driverChampId)
    {
        var driverChampionship = driverChampRepo.GetByIdWithSeries(driverChampId);
        if (driverChampionship == null ) return ResponseResult<DriverStandingsDto>.Failure("Az egyéni bajnokság nem található");
        
        var series = seriesRepo.GetSeriesById(driverChampionship.SeriesId);
        if (series == null ) return ResponseResult<DriverStandingsDto>.Failure("A széria nem található");

        var results = resultsRepo.GetByDriversChampionshipId(driverChampId);
        if (results.Count == 0)
            return ResponseResult<DriverStandingsDto>.Success(new DriverStandingsDto(driverChampId, []));
        
        var standingsResult = results
            .GroupBy(r => r.DriverId)
            .Select(group =>
            {
                var latestResult = group.Last(); 
            
                return new
                {
                    DriverId = group.Key,
                    DriverName = latestResult.Driver != null 
                        ? $"{latestResult.Driver.Name}" 
                        : "Unknown",
                    Nationality = latestResult.Driver?.Nationality ?? "N/A",
                    ConstructId = latestResult.ConstructorId,
                    ConstructorName = latestResult.Constructor?.Nickname ?? "N/A",
                    Points = group.Sum(r => r.DriverPoints),
                    BestFinish = group.Min(r => r.FinishPosition)
                };
            })
            .OrderByDescending(x => x.Points)
            .ThenBy(x => x.BestFinish)
            .Select((x, index) => new DriverStandingsResultDto(
                index + 1,
                x.DriverId,
                x.DriverName,
                x.Nationality,
                x.ConstructId,
                x.ConstructorName,
                x.Points
            ))
            .ToArray();

        return ResponseResult<DriverStandingsDto>.Success(
            new DriverStandingsDto(driverChampId, standingsResult)
        );

    }

    public ResponseResult<ConstructorStandingsDto> GetConstructorStandings(Guid constructorsChampionId)
    {
        var constructorChampionship = constructorChampRepo.GetByIdWithSeries(constructorsChampionId);
        if (constructorChampionship == null) 
            return ResponseResult<ConstructorStandingsDto>.Failure("A konstruktőri bajnokság nem található");

        var results = resultsRepo.GetByConstructorsChampionshipId(constructorsChampionId);
    
        if (results.Count == 0)
            return ResponseResult<ConstructorStandingsDto>.Success(new ConstructorStandingsDto(constructorsChampionId, []));

        var standingsResult = results
            .GroupBy(r => r.ConstructorId)
            .Select(group =>
            {
                var latestResult = group.Last(); 
        
                return new
                {
                    ConstructorId = group.Key,
                    Name = latestResult.Constructor?.Name ?? "Ismeretlen",
                    Points = group.Sum(r => r.ConstructorPoints),
                    BestFinish = group.Min(r => r.FinishPosition) 
                };
            })
            .OrderByDescending(x => x.Points)
            .ThenBy(x => x.BestFinish)
            .Select((x, index) => new ConstructorStandingsResultDto(
                index + 1, 
                x.ConstructorId,
                x.Name,
                x.Points
            ))
            .ToArray();

        return ResponseResult<ConstructorStandingsDto>.Success(
            new ConstructorStandingsDto(constructorsChampionId, standingsResult)
        );
    }

    public ResponseResult<GrandPrixResultsDto> GetGrandPrixResults(Guid grandPrixId, string session)
    {
        var gp = grandsPrixRepo.GetGrandPrixById(grandPrixId);
        if (gp == null) return ResponseResult<GrandPrixResultsDto>.Failure("A nagydíj nem létezik");

        var results = resultsRepo.GetBySession(grandPrixId, session)
            .OrderBy(r => r.FinishPosition) 
            .ToList();
        if (results.Count == 0)
        {
            return ResponseResult<GrandPrixResultsDto>.Success(new GrandPrixResultsDto(grandPrixId, session, []));
        }
        
        var leader = results.First();
        var leaderTime = leader.RaceTime;
        var leaderLaps = leader.LapsCompleted;
        
        var resultsDto = results.Select(r => new GrandRrixResultDto(
            r.FinishPosition,
            r.DriverId,
            r.CarNumber,
            r.Driver?.Name ?? "N/A",
            r.ConstructorId,
            r.Constructor?.Nickname ?? "N/A",
            FormatTimeOrStatus(r, leaderTime, leaderLaps), 
            r.DriverPoints
        )).ToArray();
        
        return ResponseResult<GrandPrixResultsDto>.Success(
            new GrandPrixResultsDto(grandPrixId, session, resultsDto)
        );
    }
    
    private static string FormatTimeOrStatus(Result result, long leaderTime, int leaderLaps)
    {
        if (!result.Status.Equals("Finished", StringComparison.CurrentCultureIgnoreCase))
        {
            return result.Status; 
        }

        if (result.FinishPosition == 1)
        {
            var t = TimeSpan.FromMilliseconds(result.RaceTime);
            return t.TotalHours >= 1 
                ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}" 
                : $"{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";
        }

        if (result.LapsCompleted < leaderLaps)
        {
            var lapsDown = leaderLaps - result.LapsCompleted;
            return $"+{lapsDown} {(lapsDown == 1 ? "Lap" : "Laps")}";
        }

        var diffMs = result.RaceTime - leaderTime;
        var d = TimeSpan.FromMilliseconds(Math.Abs(diffMs));

        return d.TotalMinutes >= 1 ? $"+{(int)d.TotalMinutes}:{d.Seconds:D2}:{d.Milliseconds:D3}s" : $"+{d.Seconds}.{d.Milliseconds:D3}s";
    }
}
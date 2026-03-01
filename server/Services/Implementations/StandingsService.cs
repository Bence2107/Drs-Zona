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
    IDriverParticipationRepository driverParticipationRepo,
    IConstructorCompetitionRepository constructorCompetitionRepo,
    ISeriesRepository seriesRepo
)
: IStandingsService
{
    public async Task<ResponseResult<DefaultFiltersDto>> GetDefaultFilters()
    {
        var allSeries = await seriesRepo.GetAllSeries();
        var series = allSeries.FirstOrDefault();
        if (series == null) return ResponseResult<DefaultFiltersDto>.Failure("Nincs elérhető széria");

        var driverChamps = await driverChampRepo.GetBySeriesId(series.Id);
        var latestChamp = driverChamps
            .OrderByDescending(c => c.Season)
            .FirstOrDefault();
        if (latestChamp == null) return ResponseResult<DefaultFiltersDto>.Failure("Nincs elérhető szezon");

        var gps = await grandsPrixRepo.GetBySeriesAndYear(series.Id, int.Parse(latestChamp.Season));
        var latestGp = gps
            .OrderByDescending(g => g.RoundNumber)
            .FirstOrDefault();
        if (latestGp == null) return ResponseResult<DefaultFiltersDto>.Failure("Nincs elérhető nagydíj");

        var sessions = await resultsRepo.GetAvailableSessionsByGrandPrixId(latestGp.Id);
        var defaultSession = sessions.Contains("Race") ? "Race" : sessions.FirstOrDefault() ?? "Race";

        return ResponseResult<DefaultFiltersDto>.Success(new DefaultFiltersDto(
            series.Id,
            latestChamp.Id,
            latestGp.Id,
            defaultSession
        ));
    }
    
    public async Task<ResponseResult<List<SeriesLookupDto>>> GetAllSeries()
    {
        var series = await seriesRepo.GetAllSeries();
        var dtoS = series.Select(s => new SeriesLookupDto(s.Id, s.Name)).ToList();
        return ResponseResult<List<SeriesLookupDto>>.Success(dtoS);
    }

    public async Task<ResponseResult<List<YearLookupDto>>> GetSeasonsBySeries(Guid seriesId)
    {
        var driversChamps = await driverChampRepo.GetBySeriesId(seriesId);
        var constructorChamps = await constructorChampRepo.GetBySeriesId(seriesId);

        var dtoS = driversChamps
            .Join(
                constructorChamps,
                d => d.Season,
                c => c.Season,
                (d, c) => new YearLookupDto(
                    d.Season.ToString(),
                    d.Id,
                    c.Id
                )
            )
            .OrderByDescending(y => y.Season)
            .ToList();

        return ResponseResult<List<YearLookupDto>>.Success(dtoS);
    }

    public async Task<ResponseResult<List<DriverLookUpDto>>> GetDriversBySeason(Guid driversChampId)
    {
        var driverParticipationsOnChampionship =
            await driverParticipationRepo.GetByChampionshipId(driversChampId);

        var dto = driverParticipationsOnChampionship.Select(dp =>
                new DriverLookUpDto(
                    Id: dp.Driver!.Id,
                    Name: dp.Driver!.Name
                )
            )
            .OrderBy(dp => dp.Name)
            .ToList();

        return ResponseResult<List<DriverLookUpDto>>.Success(dto);
    }

    public async Task<ResponseResult<List<ConstructorLookUpDto>>> GetConstructorsBySeason(Guid constructorId)
    {
        var constructorParticipationsOnChampionship =
            await constructorCompetitionRepo.GetByChampionshipId(constructorId);

        var dto = constructorParticipationsOnChampionship.Select(cp =>
                new ConstructorLookUpDto(
                    Id: cp.Constructor!.Id,
                    Name: cp.Constructor!.Name,
                    ShortName: cp.Constructor!.Nickname
                )
            )
            .OrderBy(cp => cp.Name)
            .ToList();

        return ResponseResult<List<ConstructorLookUpDto>>.Success(dto);
    }

    public async Task<ResponseResult<List<GrandPrixLookupDto>>> GetGrandsPrixByChampionship(Guid driverChampId)
    {
        var champ = await driverChampRepo.GetById(driverChampId);
        if (champ == null) return ResponseResult<List<GrandPrixLookupDto>>.Failure("Bajnokság nem található");
        
        var gps = await grandsPrixRepo.GetBySeriesAndYear(champ.SeriesId, int.Parse(champ.Season));
        var dtoS = gps.Select(g => new GrandPrixLookupDto(g.Id, g.ShortName!)).ToList();
        return ResponseResult<List<GrandPrixLookupDto>>.Success(dtoS);
    }
    
    public async Task<ResponseResult<List<string>>> GetSessionsByGrandPrix(Guid grandPrixId)
    {
        var sessions = await resultsRepo.GetAvailableSessionsByGrandPrixId(grandPrixId);
        return ResponseResult<List<string>>.Success(sessions);
    }

    public async Task<ResponseResult<DriverStandingsDto>> GetDriverStandings(Guid driverChampId)
    {
        var driverChampionship = await driverChampRepo.GetByIdWithSeries(driverChampId);
        if (driverChampionship == null ) return ResponseResult<DriverStandingsDto>.Failure("Az egyéni bajnokság nem található");
        
        var series = await seriesRepo.GetSeriesById(driverChampionship.SeriesId);
        if (series == null ) return ResponseResult<DriverStandingsDto>.Failure("A széria nem található");

        var results = await resultsRepo.GetByDriversChampionshipId(driverChampId);
        if (results.Count == 0)
            return ResponseResult<DriverStandingsDto>.Success(new DriverStandingsDto(driverChampId, []));
        
        var standingsResult = results
            .GroupBy(r => r.DriverId)
            .Select(group =>
            {
                var latestResult = group.OrderBy(r => r.GrandPrix!.RoundNumber).Last();
            
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

    public async Task<ResponseResult<ConstructorStandingsDto>> GetConstructorStandings(Guid constructorsChampionId)
    {
        var constructorChampionship = await constructorChampRepo.GetByIdWithSeries(constructorsChampionId);
        if (constructorChampionship == null) 
            return ResponseResult<ConstructorStandingsDto>.Failure("A konstruktőri bajnokság nem található");

        var results = await resultsRepo.GetByConstructorsChampionshipId(constructorsChampionId);
    
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
                    ShortName = latestResult.Constructor?.Nickname ?? "Ismeretlen",
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
                x.ShortName,
                x.Points
            ))
            .ToArray();

        return ResponseResult<ConstructorStandingsDto>.Success(
            new ConstructorStandingsDto(constructorsChampionId, standingsResult)
        );
    }

    public async Task<ResponseResult<GrandPrixResultsDto>> GetGrandPrixResults(Guid grandPrixId, string session)
    {
        var gp = await grandsPrixRepo.GetGrandPrixById(grandPrixId);
        if (gp == null) return ResponseResult<GrandPrixResultsDto>.Failure("A nagydíj nem létezik");

        var rawResults = await resultsRepo.GetBySession(grandPrixId, session);
        var results = rawResults
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
    
    public async Task<ResponseResult<List<DriverSeasonResultDto>>> GetDriverResultsBySeason(Guid driverId, Guid driverChampId)
    {
        var rawResults = await resultsRepo.GetByDriversChampionshipId(driverChampId);
        var results = rawResults
            .Where(r => r.DriverId == driverId &&  string.Equals(r.Session, "Verseny", StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.GrandPrix?.RoundNumber)
            .Select(r => new DriverSeasonResultDto(
                r.GrandPrix?.Name ?? "N/A Grand Prix",
                r.GrandPrix?.ShortName ?? "N/A GP",
                r.GrandPrix?.EndTime ?? DateTime.MinValue,
                r.Constructor?.Nickname ?? "N/A",
                r.FinishPosition,
                r.DriverPoints
            ))
            .ToList();

        return ResponseResult<List<DriverSeasonResultDto>>.Success(results);
    }
    
    public async Task<ResponseResult<List<ConstructorSeasonResultDto>>> GetConstructorResultsBySeason(Guid constructorId, Guid constructorChampId)
    {
        var rawResults = await resultsRepo.GetByConstructorsChampionshipId(constructorChampId);
        var results = rawResults
            .Where(r => r.ConstructorId == constructorId &&  string.Equals(r.Session, "Verseny", StringComparison.OrdinalIgnoreCase))
            .GroupBy(r => new { r.GrandPrixId, r.GrandPrix?.Name, r.GrandPrix?.ShortName,r.GrandPrix?.EndTime, r.GrandPrix?.RoundNumber })
            .OrderBy(g => g.Key.RoundNumber)
            .Select(group => new ConstructorSeasonResultDto(
                group.Key.Name ?? "N/A Grand Prix",
                group.Key.ShortName ?? "N/A GP",
                group.Key.EndTime ?? DateTime.MinValue,
                group.Sum(r => r.ConstructorPoints) 
            ))
            .ToList();

        return ResponseResult<List<ConstructorSeasonResultDto>>.Success(results);
    }
    
    public async Task<ResponseResult<List<SeasonOverviewDto>>> GetSeasonOverview(Guid driverChampId)
    {
        var rawResults = await resultsRepo.GetByDriversChampionshipId(driverChampId);
        var results = rawResults
            .Where(r => r.FinishPosition == 1 && 
                        string.Equals(r.Session, "Verseny", StringComparison.OrdinalIgnoreCase))
            .OrderBy(r => r.GrandPrix?.RoundNumber)
            .ToList();

        var overview = results.Select(r => new SeasonOverviewDto(
            r.GrandPrix?.Name ?? "Ismeretlen Grand Prix",
            r.GrandPrix?.ShortName ?? "Ismeretlen GP",
            r.GrandPrix?.EndTime ?? DateTime.MinValue,
            r.Driver?.Name ?? "Ismeretlen",
            r.Constructor?.Nickname ?? "N/A",
            r.LapsCompleted,
            FormatTimeOrStatus(r, r.RaceTime, r.LapsCompleted) 
        )).ToList();

        return ResponseResult<List<SeasonOverviewDto>>.Success(overview);
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
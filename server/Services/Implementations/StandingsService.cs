using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class StandingsService (
    IDriversChampionshipsRepository driverChampRepo,
    IDriversRepository driversRepo,
    IConstructorsChampionshipsRepository constructorChampRepo,
    IConstructorsRepository constructorsRepo,
    IResultsRepository resultsRepo,
    IGrandsPrixRepository grandsPrixRepo,
    IDriverParticipationRepository driverParticipationRepo,
    IConstructorCompetitionRepository constructorCompetitionRepo,
    ISeriesRepository seriesRepo,
    IContractsRepository contractsRepo,
    IQualifyingResultRepository qualifyingRepo
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
    
    public async Task<ResponseResult<ParticipationListDto>> GetParticipations(Guid driversChampId, Guid constructorsChampId)
    {
        var driverParticipations = await driverParticipationRepo.GetByChampionshipId(driversChampId);
        var constructorCompetitions = await constructorCompetitionRepo.GetByChampionshipId(constructorsChampId);

        var drivers = new List<DriverParticipationDto>();
        foreach (var dp in driverParticipations)
        {
            var contracts = await contractsRepo.GetByDriverId(dp.DriverId);
            var currentConstructorId = contracts.LastOrDefault()?.ConstructorId;
        
            var teamSnapshot = constructorCompetitions
                .FirstOrDefault(cc => cc.ConstructorId == currentConstructorId)
                ?.ConstructorNameSnapshot;

            drivers.Add(new DriverParticipationDto(
                DriverId: dp.DriverId,
                Name: dp.DriverNameSnapshot,
                Nationality: dp.Driver?.Nationality ?? "N/A",
                Age: dp.Driver != null ? GetAge(dp.Driver.BirthDate) : 0,
                DriverNumber: dp.DriverNumber,
                TeamName: teamSnapshot ?? "N/A"
            ));
        }

        var constructors = constructorCompetitions
            .Where(cc => cc.Constructor != null)
            .Select(cc => new ConstructorListDto(
                Id: cc.ConstructorId,
                Name: cc.ConstructorNameSnapshot
            )).ToList();

        return ResponseResult<ParticipationListDto>.Success(new ParticipationListDto(
            driversChampId,
            constructorsChampId,
            drivers,
            constructors
        ));
    }
    
    public async Task<ResponseResult<List<ChampionshipRowDto>>> GetAllChampionshipsBySeries(Guid seriesId)
    {
        var driversChamps = await driverChampRepo.GetBySeriesId(seriesId);
        var constChamps = await constructorChampRepo.GetBySeriesId(seriesId);

        var result = driversChamps
            .Join(constChamps,
                d => d.Season,
                c => c.Season,
                (d, c) => new ChampionshipRowDto(
                    DriversChampId: d.Id,
                    ConstructorsChampId: c.Id,
                    Season: d.Season,
                    Status: d.Status,
                    SeriesName: d.Series?.Name ?? string.Empty,
                    DriversChampName: d.Name,
                    ConstructorsChampName: c.Name
                ))
            .OrderByDescending(x => x.Season)
            .ToList();

        return ResponseResult<List<ChampionshipRowDto>>.Success(result);
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
        var driverParticipations = await driverParticipationRepo.GetByChampionshipId(driversChampId);
        var resultList = new List<DriverLookUpDto>();

        foreach (var dp in driverParticipations)
        {
            var contracts = await contractsRepo.GetByDriverId(dp.DriverId);
            var currentConstructorId = contracts.LastOrDefault()?.ConstructorId;

            resultList.Add(new DriverLookUpDto(
                Id: dp.DriverId,
                Name: dp.DriverNameSnapshot,
                ConstructorId: currentConstructorId
            ));
        }

        return ResponseResult<List<DriverLookUpDto>>.Success(
            resultList.OrderBy(d => d.Name).ToList()
        );
    }

    public async Task<ResponseResult<List<ConstructorLookUpDto>>> GetConstructorsBySeason(Guid constructorId)
    {
        var constructorParticipationsOnChampionship =
            await constructorCompetitionRepo.GetByChampionshipId(constructorId);

        var dto = constructorParticipationsOnChampionship.Select(cp =>
                new ConstructorLookUpDto(
                    Id: cp.Constructor!.Id,
                    Name: cp.ConstructorNameSnapshot,
                    ShortName: cp.ConstructorNicknameSnapshot
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
        var dtoS = gps.Select(gp =>
            {
                var hasResults = resultsRepo.HasGrandPrixResults(gp);
                var dto = new GrandPrixLookupDto(
                    gp.Id,
                    gp.ShortName!,
                    hasResults.Result,
                    gp.Circuit!.Name,
                    gp.RoundNumber,
                    gp.Circuit!.Location,
                    gp.StartTime,
                    gp.EndTime,
                    gp.RaceDistance,
                    gp.LapsCompleted
                );

                return dto;
            })
            .OrderBy(gp => gp.RoundNumber)
            .ToList();
       
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
                    DriverName = latestResult.DriverNameSnapshot,
                    Nationality = latestResult.Driver?.Nationality ?? "N/A",
                    ConstructId = latestResult.ConstructorId,
                    ConstructorName = latestResult.ConstructorNicknameSnapshot,
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
                    Name = latestResult.ConstructorNameSnapshot,
                    ShortName = latestResult.ConstructorNicknameSnapshot,
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
    
    var series = await seriesRepo.GetSeriesById(gp.SeriesId);
    if (series == null) return ResponseResult<GrandPrixResultsDto>.Failure("Széria nem található");

    var rawResults = await resultsRepo.GetBySession(grandPrixId, session);
    var results = rawResults
        .OrderBy(r => r.FinishPosition) 
        .ToList();
        
    if (results.Count == 0)
    {
        return ResponseResult<GrandPrixResultsDto>.Success(new GrandPrixResultsDto(grandPrixId, session, []));
    }

    var isF1Qualy = series.PointSystem == "F1" && (session.Contains("Időmérő") || session.Contains("Qualifying"));
    
    Dictionary<Guid, QualifyingResult>? qualyData = null;
    if (isF1Qualy)
    {
        var resultIds = results.Select(r => r.Id).ToList();
        var qualyList = await qualifyingRepo.GetByResultIds(resultIds);
        qualyData = qualyList.ToDictionary(q => q.ResultId);
    }
    
    var leader = results.First();
    var leaderTime = leader.RaceTime;
    var leaderLaps = leader.LapsCompleted;

    var resultsDto = results.Select(r => 
    {
        // Megkeressük az időmérő adatokat, ha ez egy időmérő session
        string? q1 = null;
        string? q2 = null;
        string? q3 = null;

        if (isF1Qualy && qualyData != null && qualyData.TryGetValue(r.Id, out var q))
        {
            q1 = FormatAbsoluteTime(q.Q1);
            q2 = FormatAbsoluteTime(q.Q2);
            q3 = FormatAbsoluteTime(q.Q3);
        }

        return new GrandRrixResultDto(
            r.FinishPosition,
            r.DriverId,
            r.CarNumber,
            r.DriverNameSnapshot,
            r.ConstructorId,
            r.ConstructorNicknameSnapshot,
            FormatTimeOrStatus(r, leaderTime, leaderLaps), 
            r.DriverPoints,
            q1, 
            q2, 
            q3,
            r.LapsCompleted,
            r.IsFastestLap,   
            r.IsPolePosition 
        );
    }).ToArray();

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
                r.ConstructorNicknameSnapshot,
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
            r.DriverNameSnapshot,
            r.ConstructorNicknameSnapshot,
            r.LapsCompleted,
            FormatTimeOrStatus(r, r.RaceTime, r.LapsCompleted) 
        )).ToList();

        return ResponseResult<List<SeasonOverviewDto>>.Success(overview);
    }

    public async Task<ResponseResult<bool>> CreateChampionship(ChampionshipCreateDto dto)
    {
        var driversChamp = new DriversChampionship
        {
            Id = Guid.NewGuid(),
            SeriesId = dto.SeriesId,
            Season = dto.Season,
            Name = dto.DriversName,
            Status = "Upcoming"
        };

        var constructorsChamp = new ConstructorsChampionship
        {
            Id = Guid.NewGuid(),
            SeriesId = dto.SeriesId,
            Season = dto.Season,
            Name = dto.ConstructorsName,
            Status = "Upcoming"
        };

        await driverChampRepo.Add(driversChamp);
        await constructorChampRepo.Create(constructorsChamp);

        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<bool>> UpdateChampionshipStatus(Guid driversChampId, Guid constructorsChampId, string status)
    {
        var driversChamp = await driverChampRepo.GetById(driversChampId);
        if (driversChamp == null) return ResponseResult<bool>.Failure("Egyéni bajnokság nem található");

        var constructorsChamp = await constructorChampRepo.GetByIdWithSeries(constructorsChampId);
        if (constructorsChamp == null) return ResponseResult<bool>.Failure("Konstruktőri bajnokság nem található");

        driversChamp.Status = status;
        constructorsChamp.Status = status;

        await driverChampRepo.Modify(driversChamp);
        await constructorChampRepo.Update(constructorsChamp);

        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<bool>> AddParticipations(AddParticipationsDto dto)
    {
        foreach (var constructorId in dto.ConstructorIds)
        {
            var alreadyExists = await constructorCompetitionRepo.CheckIfExists(constructorId, dto.ConstructorsChampId);
            if (alreadyExists) continue;
            
            var constructor = await constructorsRepo.GetByIdWithBrand(constructorId);
            if (constructor == null) return ResponseResult<bool>.Failure("Constructor dosen't exist");
                
            await constructorCompetitionRepo.Create(new ConstructorCompetition
            {
                ConstructorId = constructorId,
                ConstChampId = dto.ConstructorsChampId,
                ConstructorNameSnapshot = constructor.Name,
                ConstructorNicknameSnapshot = constructor.Nickname
            });
        }

        foreach (var driver in dto.Drivers)
        {
            var alreadyExists = await driverParticipationRepo.CheckIfExists(driver.DriverId, dto.DriversChampId);
            if (alreadyExists) continue;
            var driverEntity = await driversRepo.GetDriverById(driver.DriverId);
            if (driverEntity == null) return ResponseResult<bool>.Failure("Driver dosen't exist");
                
            await driverParticipationRepo.Create(new DriverParticipation
            {
                DriverId = driver.DriverId,
                DriverChampId = dto.DriversChampId,
                DriverNumber = driver.DriverNumber,
                DriverNameSnapshot = driverEntity.Name
            });
        }

        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> RemoveDriverParticipation(Guid driverId, Guid driversChampId)
    {
        await driverParticipationRepo.Delete(driverId, driversChampId);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> RemoveConstructorCompetition(Guid constructorId, Guid constructorsChampId)
    {
        await constructorCompetitionRepo.Delete(constructorId, constructorsChampId);
        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<bool>> InsertResults(BatchResultCreateDto dto) {
        var gp = await grandsPrixRepo.GetGrandPrixById(dto.GrandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");

        var isF1Qualy = series.PointSystem == "F1" && dto.Session.Contains("Időmérő");
        var leaderDto = dto.Results.FirstOrDefault(r => r.FinishPosition == 1);
        var leaderTimeMs = leaderDto != null ? ParseAbsoluteTime(leaderDto.RaceTime) : 0;

        foreach (var r in dto.Results)
        {
            var driverPart = await driverParticipationRepo.GetByDriverAndChampionship(r.DriverId, dto.DriversChampId);
            var consComp = await constructorCompetitionRepo.GetByConstructorAndChampionship(r.ConstructorId, dto.ConsChampId);
            if (driverPart == null || consComp == null) continue;

            var (driverPoints, constructorPoints) = CalculatePoints(series.PointSystem, dto.Session, r.FinishPosition, r.Pole, r.IsFastestLap);
            var startPos = await GetStartPosition(series.PointSystem, dto.Session, dto.GrandPrixId, r.DriverId, r.StartPosition);

            var existingResult = await resultsRepo.GetByGpDriverSession(dto.GrandPrixId, r.DriverId, dto.Session);

            // Kiszámoljuk az időt
            var calculatedRaceTime = r.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase) 
                ? ResolveRaceTime(r.RaceTime, leaderTimeMs) 
                : 0;

            if (existingResult == null)
            {
                var newResult = new Result
                {
                    // Required mezők
                    DriverNameSnapshot = driverPart.DriverNameSnapshot,
                    ConstructorNameSnapshot = consComp.ConstructorNameSnapshot,
                    ConstructorNicknameSnapshot = consComp.ConstructorNicknameSnapshot,
                    Session = dto.Session,
                    Status = r.Status,
                
                    // Többi mező
                    GrandPrixId = dto.GrandPrixId,
                    DriverId = r.DriverId,
                    ConstructorId = r.ConstructorId,
                    DriversChampId = dto.DriversChampId,
                    ConsChampId = dto.ConsChampId,
                    StartPosition = startPos,
                    FinishPosition = r.FinishPosition,
                    RaceTime = calculatedRaceTime,
                    DriverPoints = driverPoints,
                    ConstructorPoints = constructorPoints,
                    LapsCompleted = r.LapsCompleted,
                    CarNumber = driverPart.DriverNumber,
                    IsPolePosition = r.Pole,
                    IsFastestLap = r.IsFastestLap
                };
                await resultsRepo.Create(newResult);
                
                if (isF1Qualy) {
                    await qualifyingRepo.AddAsync(new QualifyingResult { 
                        ResultId = newResult.Id, 
                        Q1 = string.IsNullOrWhiteSpace(r.Q1) ? null : ParseAbsoluteTime(r.Q1), 
                        Q2 = string.IsNullOrWhiteSpace(r.Q2) ? null : ParseAbsoluteTime(r.Q2), 
                        Q3 = string.IsNullOrWhiteSpace(r.Q3) ? null : ParseAbsoluteTime(r.Q3) 
                    });
                }
            }
            else
            {
                existingResult.StartPosition = startPos;
                existingResult.FinishPosition = r.FinishPosition;
                existingResult.RaceTime = calculatedRaceTime;
                existingResult.DriverPoints = driverPoints;
                existingResult.ConstructorPoints = constructorPoints;
                existingResult.LapsCompleted = r.LapsCompleted;
                existingResult.Status = r.Status;
                existingResult.IsPolePosition = r.Pole;
                existingResult.IsFastestLap = r.IsFastestLap;

                await resultsRepo.Update(existingResult);

                if (isF1Qualy) {
                    var q = await qualifyingRepo.GetByResultId(existingResult.Id);
    
                    long? parsedQ1 = string.IsNullOrWhiteSpace(r.Q1) ? null : ParseAbsoluteTime(r.Q1);
                    long? parsedQ2 = string.IsNullOrWhiteSpace(r.Q2) ? null : ParseAbsoluteTime(r.Q2);
                    long? parsedQ3 = string.IsNullOrWhiteSpace(r.Q3) ? null : ParseAbsoluteTime(r.Q3);

                    if (q != null) {
                        q.Q1 = parsedQ1; 
                        q.Q2 = parsedQ2; 
                        q.Q3 = parsedQ3;
                        await qualifyingRepo.UpdateAsync(q);
                    } else {
                        await qualifyingRepo.AddAsync(new QualifyingResult { 
                            ResultId = existingResult.Id, Q1 = parsedQ1, Q2 = parsedQ2, Q3 = parsedQ3 
                        });
                    }
                }
            }
        }
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> SaveSessionResults(BatchResultCreateDto dto)
    {
        var existingResults = await resultsRepo.GetBySession(dto.GrandPrixId, dto.Session);
        foreach (var res in existingResults)
        {
            await resultsRepo.Delete(res.Id); 
        }
        return await InsertResults(dto);
    }

    public async Task<ResponseResult<GrandPrixChampionshipContextDto>> GetGrandPrixContext(Guid grandPrixId)
    {
        var gp = await grandsPrixRepo.GetGrandPrixById(grandPrixId);
        if (gp == null) return ResponseResult<GrandPrixChampionshipContextDto>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<GrandPrixChampionshipContextDto>.Failure("Széria nem található");

        var driversChamp = (await driverChampRepo.GetBySeriesId(gp.SeriesId))
            .FirstOrDefault(c => c.Season == series.LastYear.ToString());
        if (driversChamp == null) return ResponseResult<GrandPrixChampionshipContextDto>.Failure("Egyéni bajnokság nem található");

        var consChamp = (await constructorChampRepo.GetBySeriesId(gp.SeriesId))
            .FirstOrDefault(c => c.Season == series.LastYear.ToString());
        if (consChamp == null) return ResponseResult<GrandPrixChampionshipContextDto>.Failure("Konstruktőri bajnokság nem található");

        var availableSessions = series.PointSystem switch
        {
            "F1"     => new List<string> { "Sprint Időmérő", "Időmérő" ,"Sprint", "Verseny" },
            "F2"     => new List<string> { "Időmérő", "Sprint", "Verseny" },
            _        => new List<string> { "Verseny" }
        };

        return ResponseResult<GrandPrixChampionshipContextDto>.Success(
            new GrandPrixChampionshipContextDto(driversChamp.Id, consChamp.Id, series.PointSystem, availableSessions)
        );
    }

    public async Task<ResponseResult<SessionEditDto>> GetSessionForEdit(Guid grandPrixId, string session)
    {
        var gp = await grandsPrixRepo.GetGrandPrixById(grandPrixId);
        var series = await seriesRepo.GetSeriesById(gp!.SeriesId);
        bool isF1Qualy = series!.PointSystem == "F1" && session.Contains("Időmérő");

        var rawResults = await resultsRepo.GetBySession(grandPrixId, session);
        var ordered = rawResults.OrderBy(r => r.FinishPosition).ToList();
    
        if (ordered.Count == 0)
            return ResponseResult<SessionEditDto>.Success(new SessionEditDto(grandPrixId, session, []));

        var leader = ordered.First();
        var resultsDto = new List<ResultEditDto>();

        foreach (var r in ordered)
        {
            string? q1 = null, q2 = null, q3 = null;

            // Ha F1 időmérő, kikérjük a részidőket és formázzuk őket
            if (isF1Qualy)
            {
                var q = await qualifyingRepo.GetByResultId(r.Id);
                if (q != null)
                {
                    q1 = FormatAbsoluteTime(q.Q1);
                    q2 = FormatAbsoluteTime(q.Q2);
                    q3 = FormatAbsoluteTime(q.Q3);
                }
            }

            resultsDto.Add(new ResultEditDto(
                ResultId:          r.Id,
                DriverId:          r.DriverId,
                CarNumber:         r.CarNumber,
                DriverName:        r.DriverNameSnapshot,
                ConstructorId:     r.ConstructorId,
                ConstructorName:   r.ConstructorNicknameSnapshot,
                StartPosition:     r.StartPosition,
                FinishPosition:    r.FinishPosition,
                RaceTime:          FormatRaceTimeForEdit(r.RaceTime, leader.RaceTime, r.LapsCompleted, leader.LapsCompleted, r.Status, r.FinishPosition),
                LapsCompleted:     r.LapsCompleted,
                Status:            r.Status,
                DriverPoints:      r.DriverPoints,
                ConstructorPoints: r.ConstructorPoints,
                IsPole:            r.IsPolePosition,
                IsFastestLap:      r.IsFastestLap,
                Q1:                q1,
                Q2:                q2,
                Q3:                q3
            ));
        }

        return ResponseResult<SessionEditDto>.Success(new SessionEditDto(grandPrixId, session, resultsDto));
    }
    
    public async Task<ResponseResult<bool>> UpdateSingleResult(SingleResultUpdateDto dto)
    {
        var result = await resultsRepo.GetResultById(dto.ResultId);
        if (result == null) return ResponseResult<bool>.Failure("Eredmény nem található");

        var gp = await grandsPrixRepo.GetGrandPrixById(result.GrandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");

        // 1. Pontszámítás az új flag-ek alapján (F2 bónuszpontok kezelése)
        var (driverPoints, constructorPoints) = CalculatePoints(
            series.PointSystem,
            result.Session,
            dto.FinishPosition,
            dto.IsPole,         
            dto.IsFastestLap    
        );

        // 2. Idő számítása az adatbázis számára (ms)
        var allSessionResults = await resultsRepo.GetBySession(result.GrandPrixId, result.Session);
        var leader = allSessionResults.FirstOrDefault(r => r.FinishPosition == 1);
    
        // Ha az aktuálisan frissített versenyző az első, akkor az ő új idejét vesszük alapul a többieknek
        var leaderTimeMs = (dto.FinishPosition == 1) 
            ? ParseAbsoluteTime(dto.RaceTime) 
            : (leader?.RaceTime ?? 0);

        // 3. Entitás frissítése
        result.FinishPosition    = dto.FinishPosition;
        result.IsPolePosition    = dto.IsPole;          
        result.IsFastestLap      = dto.IsFastestLap;
        result.LapsCompleted     = dto.LapsCompleted;
        result.Status            = dto.Status;
        result.DriverPoints      = driverPoints;
        result.ConstructorPoints = constructorPoints;

        // Itt a kulcs: a ResolveRaceTime long-ot ad vissza, ami mehet a RaceTime-ba
        result.RaceTime = dto.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase)
            ? ResolveRaceTime(dto.RaceTime, leaderTimeMs)
            : 0;

        // 4. Rajthelyzet frissítése (pl. F2 fordított rajtrács miatt fontos lehet)
        result.StartPosition = await GetStartPosition(
            series.PointSystem,
            result.Session,
            result.GrandPrixId,
            result.DriverId,
            result.StartPosition
        );

        await resultsRepo.Update(result);

        var isF1Qualy = series.PointSystem == "F1" && result.Session.Contains("Időmérő");
        if (!isF1Qualy) return ResponseResult<bool>.Success(true);
        var qualy = await qualifyingRepo.GetByResultId(result.Id);
        
        long? parsedQ1 = string.IsNullOrWhiteSpace(dto.Q1) ? null : ParseAbsoluteTime(dto.Q1);
        long? parsedQ2 = string.IsNullOrWhiteSpace(dto.Q2) ? null : ParseAbsoluteTime(dto.Q2);
        long? parsedQ3 = string.IsNullOrWhiteSpace(dto.Q3) ? null : ParseAbsoluteTime(dto.Q3);

        if (qualy == null)
        {
            qualy = new QualifyingResult
            {
                ResultId = result.Id,
                Q1 = parsedQ1,
                Q2 = parsedQ2,
                Q3 = parsedQ3
            };
            await qualifyingRepo.AddAsync(qualy);
        }
        else
        {
            qualy.Q1 = parsedQ1;
            qualy.Q2 = parsedQ2;
            qualy.Q3 = parsedQ3;
            await qualifyingRepo.UpdateAsync(qualy);
        }

        return ResponseResult<bool>.Success(true);
    }
    
    private static (double driverPoints, double constructorPoints) CalculatePoints(
        string scoringSystem, string session, int finishPosition, bool isPole = false, bool isFastestLap = false)
    {
        return scoringSystem switch
        {
            "F1"     => CalculateF1Points(session, finishPosition), // F1-nél nem adjuk át a pole-t és a leggyorsabb kört
            "F2"     => CalculateF2Points(session, finishPosition, isPole, isFastestLap),
            _        => (0, 0)
        };
    }

    private static (double, double) CalculateF1Points(string session, int position)
    {
        var points = session switch
        {
            "Verseny" => position switch
            {
                1 => 25, 2 => 18, 3 => 15, 4 => 12, 5 => 10,
                6 => 8,  7 => 6,  8 => 4,  9 => 2,  10 => 1,
                _ => 0
            },
            "Sprint" => position switch
            {
                1 => 8, 2 => 7, 3 => 6, 4 => 5, 5 => 4,
                6 => 3, 7 => 2, 8 => 1, _ => 0
            },
            _ => 0
        };

        return (points, points); // F1-ben nincs bónuszpont
    }

    private static (double, double) CalculateF2Points(string session, int position, bool isPole, bool isFastestLap)
    {
        double driverPoints = session switch
        {
            "Verseny" => position switch
            {
                1 => 25, 2 => 18, 3 => 15, 4 => 12, 5 => 10,
                6 => 8,  7 => 6,  8 => 4,  9 => 2,  10 => 1,
                _ => 0
            },
            "Sprint" => position switch
            {
                1 => 10, 2 => 8, 3 => 6, 4 => 5, 5 => 4,
                6 => 3,  7 => 2, 8 => 1, _ => 0
            },
            _ => 0
        };

        // F2 Bónuszpontok:
        // Pole: Csak Feature (Verseny) sessionben ér 2 pontot
        if (isPole && session == "Verseny") driverPoints += 2;
        
        // Leggyorsabb kör: 1 pont, de csak ha a top 10-ben végzett
        if (isFastestLap && position <= 10) driverPoints += 1;

        return (driverPoints, driverPoints); // Konstruktőrök is megkapják
    }
    
    public async Task<ResponseResult<bool>> RecalculateSession(Guid grandPrixId, string session)
    {
        var gp = await grandsPrixRepo.GetGrandPrixById(grandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");

        var sessionResults = await resultsRepo.GetBySession(grandPrixId, session);
        if (sessionResults.Count == 0) return ResponseResult<bool>.Failure("Nincs eredmény ebben a sessionben");
        

        foreach (var result in sessionResults)
        {
            var (driverPoints, constructorPoints) = CalculatePoints(
                series.PointSystem,
                session,
                result.FinishPosition,
                result.IsPolePosition, 
                result.IsFastestLap    
            );

            result.DriverPoints      = driverPoints;
            result.ConstructorPoints = constructorPoints;
            result.StartPosition     = await GetStartPosition(
                series.PointSystem,
                session,
                grandPrixId,
                result.DriverId,
                result.StartPosition
            );

            await resultsRepo.Update(result);
        }

        return ResponseResult<bool>.Success(true);
    }
    

    private async Task<int> GetStartPosition(
        string pointSystem, string session, Guid grandPrixId, Guid driverId, int fallback)
    {
        var noStartPositionSessions = new[] 
        { 
            "Practice", "Időmérő", "Időmérő", 
            "Sprint Időmérő", 
        };
    
        if (noStartPositionSessions.Contains(session)) return 0;

        var existingSessions = await resultsRepo.GetAvailableSessionsByGrandPrixId(grandPrixId);

        var qualifyingSession = pointSystem switch
        {
            "F1" => session switch
            {
                "Sprint"  => "Sprint Időmérő",
                "Verseny" => existingSessions.Contains("Sprint Időmérő") 
                    ? "Sprint Időmérő" 
                    : "Időmérő",
                _ => null
            },
            "F2" => session switch
            {
                "Sprint"  => "Időmérő",
                "Verseny" => "Időmérő",
                _ => null
            },
            _ => null
        };

        if (qualifyingSession == null) return fallback;

        var qualifyingResults = await resultsRepo.GetBySession(grandPrixId, qualifyingSession);

        // F2 Sprint: top 10 időmérő fordított sorrendben indul
        if (pointSystem != "F2" || session != "Sprint")
            return qualifyingResults
                .FirstOrDefault(q => q.DriverId == driverId)
                ?.FinishPosition ?? fallback;
        {
            var topTen = qualifyingResults
                .OrderBy(q => q.FinishPosition)
                .Take(10)
                .ToList();

            var idx = topTen.FindIndex(q => q.DriverId == driverId);
            if (idx >= 0) return 10 - idx;

            // Top 10-en kívüli versenyzők az időmérő sorrendjében indulnak 11-től
            var outside = qualifyingResults
                .OrderBy(q => q.FinishPosition)
                .Skip(10)
                .ToList();

            var outsideIdx = outside.FindIndex(q => q.DriverId == driverId);
            return outsideIdx >= 0 ? 11 + outsideIdx : fallback;
        }

    }
    
    private static long ParseAbsoluteTime(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input == "-") return 0;
        var clean = input.Trim().TrimEnd('s');

        // H:MM:SS.mmm
        var hms = System.Text.RegularExpressions.Regex.Match(clean, @"^(\d+):(\d{2}):(\d{2})\.(\d{3})$");
        if (hms.Success)
            return (long.Parse(hms.Groups[1].Value) * 3600
                    + long.Parse(hms.Groups[2].Value) * 60
                    + long.Parse(hms.Groups[3].Value)) * 1000
                   + long.Parse(hms.Groups[4].Value);

        // M:SS.mmm
        var ms = System.Text.RegularExpressions.Regex.Match(clean, @"^(\d+):(\d{2})\.(\d{3})$");
        if (ms.Success)
            return (long.Parse(ms.Groups[1].Value) * 60
                    + long.Parse(ms.Groups[2].Value)) * 1000
                   + long.Parse(ms.Groups[3].Value);

        // SS.mmm
        var s = System.Text.RegularExpressions.Regex.Match(clean, @"^(\d+)\.(\d{3})$");
        if (s.Success)
            return long.Parse(s.Groups[1].Value) * 1000 + long.Parse(s.Groups[2].Value);

        return 0;
    }

    private static long ParseDeltaTime(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input == "-") return 0;
        var clean = input.Trim().TrimStart('+').TrimEnd('s');
        return ParseAbsoluteTime(clean);
    }

    private static long ResolveRaceTime(string input, long leaderTimeMs)
    {
        if (string.IsNullOrWhiteSpace(input) || input == "-") return 0;
        var trimmed = input.Trim();
        if (trimmed.StartsWith('+'))
            return leaderTimeMs + ParseDeltaTime(trimmed);
        return ParseAbsoluteTime(trimmed);
    }
    
    private static string? FormatAbsoluteTime(long? ms)
    {
        if (ms is null or <= 0) return null;
        var t = TimeSpan.FromMilliseconds(ms.Value);
        return t.TotalHours >= 1
            ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}"
            : $"{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";
    }

    private static string FormatRaceTimeForEdit(long ms, long leaderMs, int myLaps, int leaderLaps, string status, int position)
    {
        if (!status.Equals("Finished", StringComparison.OrdinalIgnoreCase)) return "-";
        if (ms <= 0) return "-";

        if (position == 1)
        {
            var t = TimeSpan.FromMilliseconds(ms);
            return t.TotalHours >= 1
                ? $"{(int)t.TotalHours}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}"
                : $"{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}";
        }

        if (myLaps < leaderLaps) return "-"; 

        var diff = TimeSpan.FromMilliseconds(ms - leaderMs);
        return diff.TotalMinutes >= 1
            ? $"+{(int)diff.TotalMinutes}:{diff.Seconds:D2}.{diff.Milliseconds:D3}s"
            : $"+{diff.Seconds}.{diff.Milliseconds:D3}s";
    }
    
    private static string FormatTimeOrStatus(Result result, long leaderTime, int leaderLaps)
    {
        if (!result.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase))
            return result.Status;

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
        return d.TotalMinutes >= 1
            ? $"+{(int)d.TotalMinutes}:{d.Seconds:D2}.{d.Milliseconds:D3}s"
            : $"+{d.Seconds}.{d.Milliseconds:D3}s";
    }
    
    private static int GetAge(DateTime birthDate)
    {
        var years = DateTime.Now.Year - birthDate.Year;
        if (DateTime.Now < birthDate.AddYears(years)) years--;
        return years;
    }
}
using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class StandingsService (
    IDriversChampionshipsRepository driverChampRepo,
    IDriversRepository driversRepo,
    ICarEntryRepository carEntryRepo,
    IConstructorsChampionshipsRepository constructorChampRepo,
    IConstructorsRepository constructorsRepo,
    IResultsRepository resultsRepo,
    IGrandsPrixRepository grandsPrixRepo,
    IDriverParticipationRepository driverParticipationRepo,
    IConstructorCompetitionRepository constructorCompetitionRepo,
    ISeriesRepository seriesRepo,
    IContractsRepository contractsRepo
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
            r.DriverNameSnapshot,
            r.ConstructorId,
            r.ConstructorNicknameSnapshot,
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
    
    public async Task<ResponseResult<bool>> InsertResults(BatchResultCreateDto dto)
    {
        // 1. GrandPrix lekérése → SeriesId + RaceDurationMinutes
        var gp = await grandsPrixRepo.GetGrandPrixById(dto.GrandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        // 2. Series lekérése → PointSystem
        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");

        // 3. NASCAR konstruktőr pont előkészítése
        // Gyártónként csak a legjobban végző autó kap konstruktőr pontot
        Dictionary<Guid, int> nascarConstructorBest = [];
        if (series.PointSystem == "NASCAR")
        {
            nascarConstructorBest = dto.Results
                .GroupBy(r => r.ConstructorId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(r => r.FinishPosition).First().FinishPosition
                );
        }
        
        // 4. Minden versenyző eredményének feldolgozása
        var leaderDto = dto.Results.FirstOrDefault(r => r.FinishPosition == 1);
        var leaderTimeMs = leaderDto != null ? ParseAbsoluteTime(leaderDto.RaceTime) : 0;
        
        foreach (var r in dto.Results)
        {
            var driverParticipation = await driverParticipationRepo
                .GetByDriverAndChampionship(r.DriverId, dto.DriversChampId);
            if (driverParticipation == null) continue;

            var constructorCompetition = await constructorCompetitionRepo
                .GetByConstructorAndChampionship(r.ConstructorId, dto.ConsChampId);
            if (constructorCompetition == null) continue;

            // 5. Pontszámítás
            var (driverPoints, constructorPoints) = CalculatePoints(
                series.PointSystem,
                dto.Session,
                r.FinishPosition,
                r.FinishPosition == 1,
                gp.RaceDurationMinutes
            );

            // 6. NASCAR konstruktőr pont → csak a legjobb autónál
            if (series.PointSystem == "NASCAR")
            {
                var bestPosition = nascarConstructorBest[r.ConstructorId];
                var (nascarPoints, _) = CalculateNascarPoints(r.FinishPosition);
                constructorPoints = r.FinishPosition == bestPosition ? nascarPoints : 0;
            }
        
            //7. Kideríteni kategóriák alapján az indulási pozíciót automatikusan
            var startingPosition = GetStartPosition(
                series.PointSystem,
                dto.Session,
                dto.GrandPrixId,
                r.DriverId,
                r.StartPosition
            );

            var result = new Result
            {
                GrandPrixId        = dto.GrandPrixId,
                DriverId           = r.DriverId,
                ConstructorId      = r.ConstructorId,
                DriversChampId     = dto.DriversChampId,
                ConsChampId        = dto.ConsChampId,
                StartPosition      = startingPosition.Result,
                FinishPosition     = r.FinishPosition,
                Session            = dto.Session,
                RaceTime           = r.Status
                    .Equals("Finished", StringComparison.OrdinalIgnoreCase)
                    ? ResolveRaceTime(r.RaceTime, leaderTimeMs)
                    : 0,
                DriverPoints       = driverPoints,
                ConstructorPoints  = constructorPoints,
                LapsCompleted      = r.LapsCompleted,
                Status             = r.Status,
                DriverNameSnapshot          = driverParticipation.DriverNameSnapshot,
                ConstructorNameSnapshot     = constructorCompetition.ConstructorNameSnapshot,
                ConstructorNicknameSnapshot = constructorCompetition.ConstructorNicknameSnapshot,
                CarNumber          = driverParticipation.DriverNumber
            };

            await resultsRepo.Create(result);
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

        var driversChamp = series.PointSystem != "WEC"
            ? (await driverChampRepo.GetBySeriesId(gp.SeriesId))
            .FirstOrDefault(c => c.Season == gp.SeasonYear.ToString())
            : null;

        var consChamp = (await constructorChampRepo.GetBySeriesId(gp.SeriesId))
            .FirstOrDefault(c => c.Season == gp.SeasonYear.ToString());
        if (consChamp == null)
            return ResponseResult<GrandPrixChampionshipContextDto>.Failure("Konstruktőri bajnokság nem található");

        // WEC esetén nincs drivers champ — ez nem hiba
        if (series.PointSystem != "WEC" && driversChamp == null)
            return ResponseResult<GrandPrixChampionshipContextDto>.Failure("Egyéni bajnokság nem található");

        var sessions = GetAvailableSessions(series.PointSystem);

        return ResponseResult<GrandPrixChampionshipContextDto>.Success(
            new GrandPrixChampionshipContextDto(
                DriversChampId: driversChamp?.Id,
                ConsChampId: consChamp.Id,
                PointSystem: series.PointSystem,
                AvailableSessions: sessions
            )
        );
    }

    public async Task<ResponseResult<SessionEditDto>> GetSessionForEdit(Guid grandPrixId, string session)
    {
        var rawResults = await resultsRepo.GetBySession(grandPrixId, session);
        var ordered = rawResults.OrderBy(r => r.FinishPosition).ToList();
        if (ordered.Count == 0)
            return ResponseResult<SessionEditDto>.Success(new SessionEditDto(grandPrixId, session, []));

        var leader = ordered.First();

        var results = ordered.Select(r => new ResultEditDto(
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
            ConstructorPoints: r.ConstructorPoints
        )).ToList();

        return ResponseResult<SessionEditDto>.Success(new SessionEditDto(grandPrixId, session, results));
    }
    
    public async Task<ResponseResult<bool>> UpdateSingleResult(SingleResultUpdateDto dto)
    {
        var result = await resultsRepo.GetResultById(dto.ResultId);
        if (result == null) return ResponseResult<bool>.Failure("Eredmény nem található");

        var gp = await grandsPrixRepo.GetGrandPrixById(result.GrandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");

        var (driverPoints, constructorPoints) = CalculatePoints(
            series.PointSystem,
            result.Session,
            dto.FinishPosition,
            dto.FinishPosition == 1,
            gp.RaceDurationMinutes
        );

        if (series.PointSystem == "NASCAR")
        {
            var sessionResults = await resultsRepo.GetBySession(result.GrandPrixId, result.Session);
            var bestPosition = sessionResults
                .Where(r => r.ConstructorId == result.ConstructorId && r.Id != result.Id)
                .Select(r => r.FinishPosition)
                .Append(dto.FinishPosition)
                .Min();
            var (nascarPoints, _) = CalculateNascarPoints(dto.FinishPosition);
            constructorPoints = dto.FinishPosition == bestPosition ? nascarPoints : 0;
        }

        var allSessionResults = await resultsRepo.GetBySession(result.GrandPrixId, result.Session);
        var leaderTimeMs = dto.FinishPosition == 1
            ? ParseAbsoluteTime(dto.RaceTime)
            : allSessionResults
                .Where(r => r.FinishPosition == 1 && r.Id != result.Id)
                .Select(r => r.RaceTime)
                .FirstOrDefault();

        result.FinishPosition    = dto.FinishPosition;
        result.RaceTime          = dto.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase)
            ? ResolveRaceTime(dto.RaceTime, leaderTimeMs)
            : 0;
        result.LapsCompleted     = dto.LapsCompleted;
        result.Status            = dto.Status;
        result.DriverPoints      = driverPoints;
        result.ConstructorPoints = constructorPoints;
        result.StartPosition     = await GetStartPosition(
            series.PointSystem,
            result.Session,
            result.GrandPrixId,
            result.DriverId,
            result.StartPosition
        );

        await resultsRepo.Update(result);
        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<bool>> RecalculateSession(Guid grandPrixId, string session)
    {
        var gp = await grandsPrixRepo.GetGrandPrixById(grandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");

        var sessionResults = await resultsRepo.GetBySession(grandPrixId, session);
        if (sessionResults.Count == 0) return ResponseResult<bool>.Failure("Nincs eredmény ebben a sessionben");

        // NASCAR konstruktőr legjobb pozíció előkészítése
        Dictionary<Guid, int> nascarBest = [];
        if (series.PointSystem == "NASCAR")
        {
            nascarBest = sessionResults
                .GroupBy(r => r.ConstructorId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(r => r.FinishPosition).First().FinishPosition
                );
        }

        foreach (var result in sessionResults)
        {
            var (driverPoints, constructorPoints) = CalculatePoints(
                series.PointSystem,
                session,
                result.FinishPosition,
                result.FinishPosition == 1, 
                gp.RaceDurationMinutes
            );

            if (series.PointSystem == "NASCAR")
            {
                var bestPosition = nascarBest[result.ConstructorId];
                var (nascarPoints, _) = CalculateNascarPoints(result.FinishPosition);
                constructorPoints = result.FinishPosition == bestPosition ? nascarPoints : 0;
            }

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
    
    public async Task<ResponseResult<bool>> InsertWecResults(WecBatchResultCreateDto dto)
    {
        var gp = await grandsPrixRepo.GetGrandPrixById(dto.GrandPrixId);
        if (gp == null) return ResponseResult<bool>.Failure("Nagydíj nem található");

        var series = await seriesRepo.GetSeriesById(gp.SeriesId);
        if (series == null) return ResponseResult<bool>.Failure("Széria nem található");
        if (series.PointSystem != "WEC") return ResponseResult<bool>.Failure("Ez a végpont csak WEC-hez használható");

        var leaderDto = dto.Results.FirstOrDefault(r => r.FinishPosition == 1);
        var leaderTimeMs = leaderDto != null ? ParseAbsoluteTime(leaderDto.RaceTime) : 0;

        foreach (var r in dto.Results)
        {
            var constructorCompetition = await constructorCompetitionRepo
                .GetByConstructorAndChampionship(r.ConstructorId, dto.ConsChampId);
            if (constructorCompetition == null) continue;

            // Pontszámítás — WEC-nél csak konstruktőri, versenyzői = 0
            var (_, constructorPoints) = CalculateWecPoints(r.FinishPosition, r.Pole, gp.RaceDurationMinutes);

            // Időmérőn az indulási pozíció: a Hyperpole/Qualifying result sorból olvassuk
            var startPosition = await GetWecStartPosition(dto.Session, dto.GrandPrixId, r.CarNumber);

            var result = new Result
            {
                Id                          = Guid.NewGuid(),
                GrandPrixId                 = dto.GrandPrixId,
                DriverId                    = null,             
                DriverNameSnapshot          = r.CarLabel,       
                ConstructorId               = r.ConstructorId,
                ConstructorNameSnapshot     = constructorCompetition.ConstructorNameSnapshot,
                ConstructorNicknameSnapshot = constructorCompetition.ConstructorNicknameSnapshot,
                DriversChampId              = null,              
                ConsChampId                 = dto.ConsChampId,
                StartPosition               = startPosition,
                FinishPosition              = r.FinishPosition,
                Session                     = dto.Session,
                CarNumber                   = r.CarNumber,
                RaceTime                    = r.Status.Equals("Finished", StringComparison.OrdinalIgnoreCase)
                    ? ResolveRaceTime(r.RaceTime, leaderTimeMs)
                    : 0,
                DriverPoints                = 0,                
                ConstructorPoints           = constructorPoints,
                LapsCompleted               = r.LapsCompleted,
                Status                      = r.Status,
                IsCarEntry                  = true,
            };

            await resultsRepo.Create(result);

            if (r.Drivers == null) continue;
            foreach (var driverEntry in r.Drivers)
            {
                var driver = await driversRepo.GetDriverById(driverEntry.DriverId);
                if (driver == null) continue;

                await carEntryRepo.Create(new CarEntry
                {
                    Id                  = Guid.NewGuid(),
                    ResultId            = result.Id,
                    DriverId            = driverEntry.DriverId,
                    DriverNameSnapshot  = driver.Name,
                    IsQualifier         = driverEntry.IsQualifier,
                });
            }
        }

        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<WecGrandPrixResultsDto>> GetWecGrandPrixResults(Guid grandPrixId, string session)
    {
        var rawResults = await resultsRepo.GetWecBySession(grandPrixId, session); 
        var ordered = rawResults.OrderBy(r => r.FinishPosition).ToList();

        if (ordered.Count == 0)
            return ResponseResult<WecGrandPrixResultsDto>.Success(
                new WecGrandPrixResultsDto(grandPrixId, session, []));

        var leader = ordered.First();
        var leaderTime = leader.RaceTime;
        var leaderLaps = leader.LapsCompleted;

        var rows = ordered.Select(r =>
        {
            var drivers = r.CarEntries?
                .Select(ce => new CarEntryDto(ce.DriverId, ce.DriverNameSnapshot, ce.IsQualifier))
                .ToList() ?? [];

            return new WecResultRowDto(
                Position:       r.FinishPosition,
                CarNumber:      r.CarNumber,
                CarLabel:       r.DriverNameSnapshot,
                ConstructorId:  r.ConstructorId,
                ConstructorName: r.ConstructorNicknameSnapshot,
                Drivers:        drivers,
                TimeOrCompleted: FormatTimeOrStatus(r, leaderTime, leaderLaps),
                Points:         r.ConstructorPoints
            );
        }).ToArray();

        return ResponseResult<WecGrandPrixResultsDto>.Success(
            new WecGrandPrixResultsDto(grandPrixId, session, rows));
    }
    
    private async Task<int> GetWecStartPosition(string session, Guid grandPrixId, int carNumber)
    {
        if (session is "Qualifying" or "Hyperpole") return 0;
        if (session != "Verseny") return 0;

        var existingSessions = await resultsRepo.GetAvailableSessionsByGrandPrixId(grandPrixId);

        var qualifyingSession = existingSessions.Contains("Hyperpole")
            ? "Hyperpole"
            : existingSessions.Contains("Qualifying")
                ? "Qualifying"
                : null;

        if (qualifyingSession == null) return 0;

        var qualResults = await resultsRepo.GetBySession(grandPrixId, qualifyingSession);
        return qualResults
            .FirstOrDefault(r => r.CarNumber == carNumber)
            ?.FinishPosition ?? 0;
    }

    public async Task<ResponseResult<bool>> SaveWecSessionResults(WecBatchResultCreateDto dto)
    {
        var existing = await resultsRepo.GetBySession(dto.GrandPrixId, dto.Session);
        foreach (var res in existing)
            await resultsRepo.Delete(res.Id);

        return await InsertWecResults(dto);
    }
    
    private static (double driverPoints, double constructorPoints) CalculatePoints(
        string scoringSystem, string session, int finishPosition, bool pole = false, int? raceDurationMinutes = null)
    {
        return scoringSystem switch
        {
            "F1"     => CalculateF1Points(session, finishPosition),
            "F2"     => CalculateF2Points(session, finishPosition, pole),
            "WEC"    => CalculateWecPoints(finishPosition, pole, raceDurationMinutes),
            "NASCAR" => CalculateNascarPoints(finishPosition),
            _        => (0, 0)
        };
    }
    
    
    private static List<SessionDefinition> GetAvailableSessions(string pointSystem) =>
        pointSystem switch
        {
            "F1" => [
                new SessionDefinition("Időmérő",          CountsForPoints: false, IsQualifying: true),
                new SessionDefinition("Sprint Qualifying",CountsForPoints: false, IsQualifying: true),
                new SessionDefinition("Sprint",           CountsForPoints: true,  IsQualifying: false),
                new SessionDefinition("Verseny",          CountsForPoints: true,  IsQualifying: false),
            ],
            "F2" => [
                new SessionDefinition("Időmérő",          CountsForPoints: false, IsQualifying: true),
                new SessionDefinition("Sprint",           CountsForPoints: true,  IsQualifying: false),
                new SessionDefinition("Verseny",          CountsForPoints: true,  IsQualifying: false),
            ],
            "WEC" => [
                new SessionDefinition("Qualifying",       CountsForPoints: false, IsQualifying: true),
                new SessionDefinition("Hyperpole",        CountsForPoints: false, IsQualifying: true),
                new SessionDefinition("Verseny",          CountsForPoints: true,  IsQualifying: false),
            ],
            "NASCAR" => [
                new SessionDefinition("Qualifying",       CountsForPoints: false, IsQualifying: true),
                new SessionDefinition("Verseny",          CountsForPoints: true,  IsQualifying: false),
            ],
            _ => [
                new SessionDefinition("Verseny",          CountsForPoints: true,  IsQualifying: false),
            ]
        };

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

        return (points, points);
    }

    private static (double, double) CalculateF2Points(string session, int position, bool pole)
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

        // Pole csak Feature (Verseny) sessionben ér 2 pontot
        if (pole && session == "Verseny") driverPoints += 2;

        return (driverPoints, driverPoints);
    }

    private static (double, double) CalculateWecPoints(int position, bool pole, int? durationMinutes)
    {
        // WEC-nél nincs session alapú különbség — minden futam egyforma,
        // csak az időtartam határozza meg a ponttáblát
        double[] sixHour   = [25, 18, 15, 12, 10, 8, 6, 4, 2, 1];
        double[] eightHour = [38, 27, 23, 18, 15, 12, 9, 6, 3, 2];
        double[] leMans    = [50, 36, 30, 24, 20, 16, 12, 8, 4, 2];

        var table = durationMinutes switch
        {
            >= 1440 => leMans,
            >= 480  => eightHour,
            _       => sixHour
        };

        // Top 10 → táblából, 11+ klasszifikált → 0.5, nem klasszifikált → 0
        var points = position is >= 1 and <= 10
            ? table[position - 1]
            : position > 10 ? 0.5 : 0;

        // Pole position = +1 pont kategóriánként
        if (pole) points += 1;

        return (points, points);
    }

    private static (double, double) CalculateNascarPoints(int position)
    {
        // 2026-os rendszer
        double driverPoints = position switch
        {
            1  => 55, 2  => 35, 3  => 34, 4  => 33, 5  => 32,
            6  => 31, 7  => 30, 8  => 29, 9  => 28, 10 => 27,
            11 => 26, 12 => 25, 13 => 24, 14 => 23, 15 => 22,
            16 => 21, 17 => 20, 18 => 19, 19 => 18, 20 => 17,
            21 => 16, 22 => 15, 23 => 14, 24 => 13, 25 => 12,
            26 => 11, 27 => 10, 28 => 9,  29 => 8,  30 => 7,
            _ => 0
        };

        return (driverPoints, 0);
    }

    private async Task<int> GetStartPosition(
        string pointSystem, string session, Guid grandPrixId, Guid? driverId, int fallback)
    {
        var noStartPositionSessions = new[] 
        { 
            "Practice", "Időmérő", "Qualifying", 
            "Sprint Qualifying", "Hyperpole" 
        };
    
        if (noStartPositionSessions.Contains(session)) return 0;

        var existingSessions = await resultsRepo.GetAvailableSessionsByGrandPrixId(grandPrixId);

        var qualifyingSession = pointSystem switch
        {
            "F1" => session switch
            {
                "Sprint"  => "Sprint Qualifying",
                "Verseny" => existingSessions.Contains("Sprint Qualifying") 
                    ? "Sprint Qualifying" 
                    : "Időmérő",
                _ => null
            },
            "F2" => session switch
            {
                "Sprint"  => "Időmérő",
                "Verseny" => "Időmérő",
                _ => null
            },
            "WEC" => session switch
            {
                "Verseny" => existingSessions.Contains("Hyperpole") 
                    ? "Hyperpole" 
                    : "Qualifying",
                _ => null
            },
            "NASCAR" => session switch
            {
                "Verseny" => "Qualifying",
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
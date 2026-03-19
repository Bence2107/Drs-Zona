using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.RaceTracks;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Types;

namespace Services.Implementations;

public class ChampionshipService(
    IConstructorsRepository constructorsRepo,
    IConstructorsChampionshipsRepository constructorChampRepo,
    IConstructorCompetitionRepository constructorCompetitionRepo,
    IContractsRepository contractsRepo,
    IDriversRepository driversRepo,
    IDriversChampionshipsRepository driverChampRepo,
    IDriverParticipationRepository driverParticipationRepo,
    IGrandsPrixRepository grandsPrixRepo,
    IResultsRepository resultsRepo
) : IChampionshipService 
{
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

        await driverChampRepo.Create(driversChamp);
        await constructorChampRepo.Create(constructorsChamp);

        return ResponseResult<bool>.Success(true);
    }
    
    public async Task<ResponseResult<bool>> CreateParticipations(ParticipationAddDto dto)
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
    
    public async Task<ResponseResult<bool>> DeleteDriverParticipation(Guid driverId, Guid driversChampId)
    {
        await driverParticipationRepo.Delete(driverId, driversChampId);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> DeleteConstructorCompetition(Guid constructorId, Guid constructorsChampId)
    {
        await constructorCompetitionRepo.Delete(constructorId, constructorsChampId);
        return ResponseResult<bool>.Success(true);
    }
    
    private static int GetAge(DateTime birthDate)
    {
        var years = DateTime.Now.Year - birthDate.Year;
        if (DateTime.Now < birthDate.AddYears(years)) years--;
        return years;
    }
}
using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class DriverService(
    IDriversRepository driverRepo, 
    IDriverParticipationRepository driverParticipationRepo,
    IContractsRepository contractsDepo
): IDriverService
{
    public ResponseResult<DriverDetailDto> GetDriverById(Guid id)
    {
        var driver = driverRepo.GetDriverById(id);
        if (driver == null) return ResponseResult<DriverDetailDto>.Failure("Driver not found.");

        var contractIds = contractsDepo.GetByDriverId(id).Select(c => c.Id).ToList();

        return ResponseResult<DriverDetailDto>.Success(new DriverDetailDto(
            driver.Id,
            driver.Name,
            driver.Nationality,
            driver.BirthDate,
            contractIds,
            driver.TotalRaces,
            driver.Wins,
            driver.Podiums,
            driver.Championships,
            driver.PolePositions,
            GetDriversAge(driver.BirthDate)
        ));
    }

    public ResponseResult<List<DriverListDto>> ListAllDriversByChampionships(Guid championshipId)
    {
        var drivers = driverParticipationRepo.GetDriversByChampionship(championshipId);
        if (drivers.Count == 0)
        {
            return ResponseResult<List<DriverListDto>>.Success([]);
        }

        var dto = drivers.Select(d =>
        {
            var currentContracts = contractsDepo.GetByDriverId(d!.Id);
            var currentTeamName = currentContracts.LastOrDefault()?.Constructor?.Name;

            return new DriverListDto(
                d.Id,
                d.Name,
                d.Nationality,
                GetDriversAge(d.BirthDate),
                currentTeamName
            );
        }).ToList();


        return ResponseResult<List<DriverListDto>>.Success(dto);
    }

    public ResponseResult<bool> CreateDriver(DriverCreateDto dto)
    {
        if (dto.BirthDate.Year > 2005)
        {
            return ResponseResult<bool>.Failure(nameof(dto.BirthDate), "Driver cannot be younger than 15 year");
        }

        if (dto.BirthDate > DateTime.Now)
        {
            return ResponseResult<bool>.Failure("Birth date cannot be in the future.");
        }

        var driver = new Driver
        {
            Name = dto.Name,
            Nationality = dto.Nationality,
            BirthDate = dto.BirthDate,
            TotalRaces = dto.TotalRaces,
            Wins = dto.TotalWins,
            Podiums = dto.TotalPodiums,
            Championships = dto.Championships,
            PolePositions = dto.PolePositions,
            Seasons = dto.Seasons
        };

        driverRepo.Create(driver);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateDriver(DriverUpdateDto dto)
    {
        var driver = driverRepo.GetDriverById(dto.Id);
        if (driver == null) return ResponseResult<bool>.Failure("Driver not found.");
        
        if (dto.BirthDate.Year > 2005)
        {
            return ResponseResult<bool>.Failure(nameof(driver.BirthDate), "Driver cannot be younger than 15 year");
        }

        if (dto.BirthDate > DateTime.Now)
        {
            return ResponseResult<bool>.Failure("Birth date cannot be in the future.");
        }

        driver.Id = dto.Id;
        driver.Name = dto.Name;
        driver.Nationality = dto.Nationality;
        driver.BirthDate = dto.BirthDate;
        driver.TotalRaces = dto.TotalRaces;
        driver.Wins = dto.TotalWins;
        driver.Podiums = dto.TotalPodiums;
        driver.Championships = dto.Championships;
        driver.PolePositions = dto.PolePositions;
        driver.Seasons = dto.Seasons;

        driverRepo.Update(driver);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> DeleteDriver(Guid id)
    {
        var driver = driverRepo.GetDriverById(id);
        if (driver == null) return ResponseResult<bool>.Failure("Driver not found.");

        driverRepo.Delete(driver.Id);
        return ResponseResult<bool>.Success(true);
    }

    private static int GetDriversAge(DateTime birthDate)
    {
        var years = DateTime.Now.Year - birthDate.Year;
        if (DateTime.Now < birthDate.AddYears(years))
        {
            years--;
        }

        return years;
    }
}
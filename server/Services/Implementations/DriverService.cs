using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;
using Services.Types;

namespace Services.Implementations;

public class DriverService(
    IDriversRepository driverRepo, 
    IContractsRepository contractsDepo
) : IDriverService 
{
    public async Task<ResponseResult<DriverDetailDto>> GetDriverById(Guid id)
    {
        var driver = await driverRepo.GetDriverById(id);
        if (driver == null) return ResponseResult<DriverDetailDto>.Failure("Driver not found.");

        var contracts = await contractsDepo.GetByDriverId(id);
        var contractIds = contracts.Select(c => c.Id).ToList();
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
            GetDriversAge(driver.BirthDate),
            driver.Seasons
        ));
    }
    
    public async Task<ResponseResult<List<DriverListDto>>> GetAllDrivers()
    {
        var drivers = await driverRepo.GetAllDrivers();
    
        var dto = new List<DriverListDto>();
        foreach (var d in drivers)
        {
            var contracts = await contractsDepo.GetByDriverId(d.Id);
            var currentTeamName = contracts.LastOrDefault()?.Constructor?.Name;
        
            dto.Add(new DriverListDto(
                d.Id,
                d.Name,
                d.Nationality,
                GetDriversAge(d.BirthDate),
                currentTeamName
            ));
        }

        var orderedDto = dto
            .OrderBy(dt => dt.Name)
            .ToList();
        return ResponseResult<List<DriverListDto>>.Success(orderedDto);
    }
    

    public async Task<ResponseResult<bool>> Create(DriverCreateDto dto)
    {
        if (dto.BirthDate > DateTime.Today.AddYears(-15))
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

        await driverRepo.Create(driver);
        return ResponseResult<bool>.Success(true);
    }

    public async Task<ResponseResult<bool>> Update(DriverUpdateDto dto)
    {
        var driver = await driverRepo.GetDriverById(dto.Id);
        if (driver == null) return ResponseResult<bool>.Failure("Driver not found.");
        
        if (dto.BirthDate > DateTime.Today.AddYears(-15))
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

        await driverRepo.Update(driver);
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
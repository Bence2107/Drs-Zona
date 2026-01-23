using DTOs.Standings;
using Entities.Models.Standings;
using Repositories.Interfaces.Standings;
using Services.Interfaces;

namespace Services.Implementations;

public class ConstructorsService(
    IConstructorsRepository constructorRepo, 
    IConstructorCompetitionRepository constructorCompetitionRepo,
    IBrandsRepository brandsRepo,
    IContractsRepository contractsRepo
) : IConstructorsService
{
    public ResponseResult<ConstructorDetailDto> GetById(int id)
    {
        if (constructorRepo.CheckIfIdExists(id)) return ResponseResult<ConstructorDetailDto>.Failure("Constructor not found");

        var constructor = constructorRepo.GetByIdWithBrand(id);
        if (constructor is { Brand: null }) return ResponseResult<ConstructorDetailDto>.Failure("Brand is invalid");
        if (constructor != null && !brandsRepo.CheckIfIdExists(constructor.BrandId)) return ResponseResult<ConstructorDetailDto>.Failure("Brand not found");

        var contracts = contractsRepo.GetByTeamId(id);
        var driverNameRecord = new List<DriverNameRecord>();
        if (contracts.Count > 0)
        {
            driverNameRecord.AddRange(contracts.Select(contract => new DriverNameRecord(Id: contract.DriverId, Name: contract.Driver!.Name)));
        } 
        
        var dto = new ConstructorDetailDto(
            Id:  constructor!.Id,
            BrandId: constructor.BrandId,
            BrandName: constructor.Brand!.Name,
            BrandDescription: constructor.Brand.Description,
            Name: constructor.Name,
            FoundedYear: constructor.FoundedYear,
            HeadQuarters: constructor.HeadQuarters,
            TeamChief: constructor.TeamChief,
            TechnicalChief: constructor.TechnicalChief,
            TotalPodiums: constructor.Podiums,
            TotalWins: constructor.Wins,
            Championships: constructor.Championships,
            DriverNames: driverNameRecord
        );
        
        return ResponseResult<ConstructorDetailDto>.Success(dto);
    }

    public ResponseResult<List<ConstructorListDto>> ListAllConstructorsByChampionship(int championshipId)
    {
        var constructors = constructorCompetitionRepo.GetConstructorsByChampionshipId(championshipId);
        if (constructors.Count == 0) return ResponseResult<List<ConstructorListDto>>.Failure("Championship does not have any constructors");
        
        var dto = constructors.Select(c => new ConstructorListDto(
            Id: c!.Id,
            Name: c.Name!
        ))
        .ToList();

        return ResponseResult<List<ConstructorListDto>>.Success(dto);
    }

    public ResponseResult<bool> CreateConstructor(ConstructorCreateDto constructorCreateDto)
    {
        var existingConstructor = constructorRepo.GetByName(constructorCreateDto.Name);
        if (existingConstructor != null) return ResponseResult<bool>.Failure("Constructor already is in the Championship");
        
        var constructor = new Constructor
        {
            Name = constructorCreateDto.Name,
            FoundedYear = constructorCreateDto.FoundedYear,
            HeadQuarters = constructorCreateDto.HeadQuarters,
            TeamChief = constructorCreateDto.TeamChief,
            TechnicalChief = constructorCreateDto.TechnicalChief,
            Wins = constructorCreateDto.Wins,
            Championships = constructorCreateDto.Championships,
            Podiums = constructorCreateDto.Podiums,
            Seasons = constructorCreateDto.Seasons
        };

        if (brandsRepo.CheckIfIdExists(constructorCreateDto.BrandId))
        {
            constructor.BrandId = constructor.BrandId;
        }

        constructorRepo.Create(constructor);
        return ResponseResult<bool>.Success(true);
    }

    public ResponseResult<bool> UpdateConstructor(ConstructorUpdateDto constructorUpdateDto)
    {
        var constructor = constructorRepo.GetConstructorById(constructorUpdateDto.Id);
        if (constructor == null) return ResponseResult<bool>.Failure("Constructor not found");

        if (constructorUpdateDto.BrandId != constructor.BrandId)
        {
            if (!brandsRepo.CheckIfIdExists(constructorUpdateDto.BrandId))
            {
                return ResponseResult<bool>.Failure("Brand not found");
            }
        }
        
        constructor.Id = constructorUpdateDto.Id;
        constructor.BrandId = constructorUpdateDto.BrandId;
        constructor.Name = constructorUpdateDto.Name;
        constructor.FoundedYear = constructorUpdateDto.FoundedYear;
        constructor.HeadQuarters = constructorUpdateDto.HeadQuarters;
        constructor.TeamChief = constructorUpdateDto.TeamChief;
        constructor.TechnicalChief = constructorUpdateDto.TechnicalChief;
        constructor.Wins = constructorUpdateDto.Wins;
        constructor.Championships = constructorUpdateDto.Championships;
        constructor.Podiums = constructorUpdateDto.Podiums;
        constructor.Seasons = constructorUpdateDto.Seasons;
        
        
        constructorRepo.Update(constructor);
        return ResponseResult<bool>.Success(true);
    }
}